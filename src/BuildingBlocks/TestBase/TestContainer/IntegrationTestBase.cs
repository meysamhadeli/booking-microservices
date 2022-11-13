using Ardalis.GuardClauses;
using BuildingBlocks.Core.Event;
using BuildingBlocks.Core.Model;
using BuildingBlocks.EFCore;
using BuildingBlocks.MassTransit;
using BuildingBlocks.Mongo;
using BuildingBlocks.PersistMessageProcessor;
using BuildingBlocks.PersistMessageProcessor.Data;
using BuildingBlocks.Web;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using Grpc.Net.Client;
using MassTransit;
using MassTransit.Testing;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NSubstitute;
using Serilog;
using Xunit;
using Xunit.Abstractions;

namespace BuildingBlocks.TestBase.TestContainer;

public class IntegrationTestFactory<TEntryPoint> : IAsyncLifetime
    where TEntryPoint : class
{
    private readonly WebApplicationFactory<TEntryPoint> _factory;
    private int Timeout => 180;
    public Action<IServiceCollection> TestRegistrationServices { set; get; }
    public HttpClient HttpClient => _factory.CreateClient();
    public ITestHarness TestHarness => CreateHarness();
    public GrpcChannel Channel => CreateChannel();

    public IServiceProvider ServiceProvider => _factory.Services;
    public IConfiguration Configuration => _factory.Services.GetRequiredService<IConfiguration>();
    public TestcontainerDatabase ContainerSqlDatabase;
    public TestcontainerDatabase ContainerSqlPersistDatabase;
    public TestcontainerDatabase ContainerMongoDatabase;
    public string MongoConnectionString;
    public string SqlConnectionString;
    public string SqlPersistConnectionString;

    public IntegrationTestFactory()
    {
        ContainerSqlDatabase = new TestcontainersBuilder<MsSqlTestcontainer>()
            .WithDatabase(new MsSqlTestcontainerConfiguration {Database = "sql_test_db", Password = "localpassword#123uuuuu"})
            .WithImage("mcr.microsoft.com/mssql/server:2017-latest")
            .WithCleanUp(true)
            .Build();

        ContainerSqlPersistDatabase = new TestcontainersBuilder<MsSqlTestcontainer>()
            .WithDatabase(new MsSqlTestcontainerConfiguration {Database = "sql_test_persist_db", Password = "localpassword#123oooo",})
            .WithImage("mcr.microsoft.com/mssql/server:2017-latest")
            .WithCleanUp(true)
            .Build();

        ContainerMongoDatabase = new TestcontainersBuilder<MongoDbTestcontainer>()
            .WithDatabase(new MongoDbTestcontainerConfiguration() {Database = "mongo_test_db", Username = "mongo_db", Password = "mongo_db_pass"})
            .WithImage("mongo")
            .WithCleanUp(true)
            .Build();

        _factory = new WebApplicationFactory<TEntryPoint>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("test");
                builder.ConfigureServices(services =>
                {
                    TestRegistrationServices?.Invoke(services);
                    services.ReplaceSingleton(AddHttpContextAccessorMock);
                    services.AddMassTransitTestHarness(x =>
                    {
                        x.UsingRabbitMq((context, cfg) =>
                        {
                            var rabbitMqOptions = services.GetOptions<RabbitMqOptions>("RabbitMq");
                            var host = rabbitMqOptions.HostName;

                            cfg.Host(host, h =>
                            {
                                h.Username(rabbitMqOptions.UserName);
                                h.Password(rabbitMqOptions.Password);
                            });
                            cfg.ConfigureEndpoints(context);
                        });
                    });
                });
            });
    }

    public async Task InitializeAsync()
    {
        await ContainerSqlDatabase.StartAsync();
        await ContainerSqlPersistDatabase.StartAsync();
        await ContainerMongoDatabase.StartAsync();

        MongoConnectionString = ContainerMongoDatabase?.ConnectionString;
        SqlConnectionString = ContainerSqlDatabase?.ConnectionString;
        SqlPersistConnectionString = ContainerSqlPersistDatabase?.ConnectionString;
    }

    public async Task DisposeAsync()
    {
        await ContainerSqlDatabase.StopAsync();
        await ContainerSqlPersistDatabase.StopAsync();
        await ContainerMongoDatabase.StopAsync();

        await _factory.DisposeAsync();
    }

    public virtual void RegisterServices(Action<IServiceCollection> services)
    {
        TestRegistrationServices = services;
    }

    // ref: https://github.com/trbenning/serilog-sinks-xunit
    public ILogger CreateLogger(ITestOutputHelper output)
    {
        if (output != null)
        {
            return new LoggerConfiguration()
                .WriteTo.TestOutput(output)
                .CreateLogger();
        }

        return null;
    }

    public async Task ExecuteScopeAsync(Func<IServiceProvider, Task> action)
    {
        using var scope = ServiceProvider.CreateScope();
        await action(scope.ServiceProvider);
    }

    public async Task<T> ExecuteScopeAsync<T>(Func<IServiceProvider, Task<T>> action)
    {
        using var scope = ServiceProvider.CreateScope();

        var result = await action(scope.ServiceProvider);

        return result;
    }

    public Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request)
    {
        return ExecuteScopeAsync(sp =>
        {
            var mediator = sp.GetRequiredService<IMediator>();

            return mediator.Send(request);
        });
    }

    public Task SendAsync(IRequest request)
    {
        return ExecuteScopeAsync(sp =>
        {
            var mediator = sp.GetRequiredService<IMediator>();

            return mediator.Send(request);
        });
    }

    // Ref: https://tech.energyhelpline.com/in-memory-testing-with-masstransit/
    public async ValueTask WaitUntilConditionMet(Func<Task<bool>> conditionToMet, int? timeoutSecond = null)
    {
        var time = timeoutSecond ?? Timeout;

        var startTime = DateTime.Now;
        var timeoutExpired = false;
        var meet = await conditionToMet.Invoke();
        while (!meet)
        {
            if (timeoutExpired) throw new TimeoutException("Condition not met for the test.");

            await Task.Delay(100);
            meet = await conditionToMet.Invoke();
            timeoutExpired = DateTime.Now - startTime > TimeSpan.FromSeconds(time);
        }
    }

    public async ValueTask ShouldProcessedPersistInternalCommand<TInternalCommand>()
        where TInternalCommand : class, IInternalCommand
    {
        await WaitUntilConditionMet(async () =>
        {
            return await ExecuteScopeAsync(async sp =>
            {
                var persistMessageProcessor = sp.GetService<IPersistMessageProcessor>();
                Guard.Against.Null(persistMessageProcessor, nameof(persistMessageProcessor));

                var filter = await persistMessageProcessor.GetByFilterAsync(x =>
                    x.DeliveryType == MessageDeliveryType.Internal &&
                    typeof(TInternalCommand).ToString() == x.DataType);

                var res = filter.Any(x => x.MessageStatus == MessageStatus.Processed);

                return res;
            });
        });
    }

    private ITestHarness CreateHarness()
    {
        var harness = ServiceProvider.GetTestHarness();
        return harness;
    }

    private GrpcChannel CreateChannel()
    {
        return GrpcChannel.ForAddress(HttpClient.BaseAddress!, new GrpcChannelOptions {HttpClient = HttpClient});
    }

    private IHttpContextAccessor AddHttpContextAccessorMock(IServiceProvider serviceProvider)
    {
        var httpContextAccessorMock = Substitute.For<IHttpContextAccessor>();
        using var scope = serviceProvider.CreateScope();
        httpContextAccessorMock.HttpContext = new DefaultHttpContext {RequestServices = scope.ServiceProvider};

        httpContextAccessorMock.HttpContext.Request.Host = new HostString("localhost", 6012);
        httpContextAccessorMock.HttpContext.Request.Scheme = "http";

        return httpContextAccessorMock;
    }
}

public class IntegrationTestFactory<TEntryPoint, TWContext> : IntegrationTestFactory<TEntryPoint>
    where TEntryPoint : class
    where TWContext : DbContext
{
    public Task ExecuteDbContextAsync(Func<TWContext, Task> action)
    {
        return ExecuteScopeAsync(sp => action(sp.GetService<TWContext>()));
    }

    public Task ExecuteDbContextAsync(Func<TWContext, ValueTask> action)
    {
        return ExecuteScopeAsync(sp => action(sp.GetService<TWContext>()).AsTask());
    }

    public Task ExecuteDbContextAsync(Func<TWContext, IMediator, Task> action)
    {
        return ExecuteScopeAsync(sp => action(sp.GetService<TWContext>(), sp.GetService<IMediator>()));
    }

    public Task<T> ExecuteDbContextAsync<T>(Func<TWContext, Task<T>> action)
    {
        return ExecuteScopeAsync(sp => action(sp.GetService<TWContext>()));
    }

    public Task<T> ExecuteDbContextAsync<T>(Func<TWContext, ValueTask<T>> action)
    {
        return ExecuteScopeAsync(sp => action(sp.GetService<TWContext>()).AsTask());
    }

    public Task<T> ExecuteDbContextAsync<T>(Func<TWContext, IMediator, Task<T>> action)
    {
        return ExecuteScopeAsync(sp => action(sp.GetService<TWContext>(), sp.GetService<IMediator>()));
    }

    public Task InsertAsync<T>(params T[] entities) where T : class
    {
        return ExecuteDbContextAsync(db =>
        {
            foreach (var entity in entities) db.Set<T>().Add(entity);

            return db.SaveChangesAsync();
        });
    }

    public async Task InsertAsync<TEntity>(TEntity entity) where TEntity : class
    {
        await ExecuteDbContextAsync(db =>
        {
            db.Set<TEntity>().Add(entity);

            return db.SaveChangesAsync();
        });
    }

    public Task InsertAsync<TEntity, TEntity2>(TEntity entity, TEntity2 entity2)
        where TEntity : class
        where TEntity2 : class
    {
        return ExecuteDbContextAsync(db =>
        {
            db.Set<TEntity>().Add(entity);
            db.Set<TEntity2>().Add(entity2);

            return db.SaveChangesAsync();
        });
    }

    public Task InsertAsync<TEntity, TEntity2, TEntity3>(TEntity entity, TEntity2 entity2, TEntity3 entity3)
        where TEntity : class
        where TEntity2 : class
        where TEntity3 : class
    {
        return ExecuteDbContextAsync(db =>
        {
            db.Set<TEntity>().Add(entity);
            db.Set<TEntity2>().Add(entity2);
            db.Set<TEntity3>().Add(entity3);

            return db.SaveChangesAsync();
        });
    }

    public Task InsertAsync<TEntity, TEntity2, TEntity3, TEntity4>(TEntity entity, TEntity2 entity2, TEntity3 entity3,
        TEntity4 entity4)
        where TEntity : class
        where TEntity2 : class
        where TEntity3 : class
        where TEntity4 : class
    {
        return ExecuteDbContextAsync(db =>
        {
            db.Set<TEntity>().Add(entity);
            db.Set<TEntity2>().Add(entity2);
            db.Set<TEntity3>().Add(entity3);
            db.Set<TEntity4>().Add(entity4);

            return db.SaveChangesAsync();
        });
    }

    public Task<T> FindAsync<T>(long id)
        where T : class, IAudit
    {
        return ExecuteDbContextAsync(db => db.Set<T>().FindAsync(id).AsTask());
    }
}

public class
    IntegrationTestFactory<TEntryPoint, TWContext, TRContext, PContext> : IntegrationTestFactory<TEntryPoint, TWContext,
        TRContext>
    where TEntryPoint : class
    where TWContext : DbContext
    where TRContext : MongoDbContext
    where PContext : PersistMessageDbContext
{

}

public class IntegrationTestFactory<TEntryPoint, TWContext, TRContext> : IntegrationTestFactory<
    TEntryPoint,
    TWContext>
    where TEntryPoint : class
    where TWContext : DbContext
    where TRContext : MongoDbContext
{
    public Task ExecuteReadContextAsync(Func<TRContext, Task> action)
    {
        return ExecuteScopeAsync(sp => action(sp.GetRequiredService<TRContext>()));
    }

    public Task<T> ExecuteReadContextAsync<T>(Func<TRContext, Task<T>> action)
    {
        return ExecuteScopeAsync(sp => action(sp.GetRequiredService<TRContext>()));
    }
}

public class IntegrationTestFixtureCore<TEntryPoint> : IAsyncLifetime
    where TEntryPoint : class
{
    private string MongoConnectionString
    {
        get => Fixture.ServiceProvider.GetRequiredService<IOptions<MongoOptions>>()?.Value?.ConnectionString;
        set => Fixture.ServiceProvider.GetRequiredService<IOptions<MongoOptions>>().Value.ConnectionString = value;
    }

    private string PersistConnectionString
    {
        get => Fixture.ServiceProvider.GetRequiredService<IOptions<PersistMessageOptions>>()?.Value.ConnectionString;
        set => Fixture.ServiceProvider.GetRequiredService<IOptions<PersistMessageOptions>>().Value.ConnectionString = value;
    }

    private string DefaultConnectionString
    {
        get => Fixture.ServiceProvider.GetRequiredService<IOptions<ConnectionStrings>>()?.Value.DefaultConnection;
        set => Fixture.ServiceProvider.GetRequiredService<IOptions<ConnectionStrings>>().Value.DefaultConnection = value;
    }

    public IntegrationTestFixtureCore(IntegrationTestFactory<TEntryPoint> integrationTestFixture)
    {
        Fixture = integrationTestFixture;
        integrationTestFixture.RegisterServices(services => RegisterTestsServices(services));
    }

    public IntegrationTestFactory<TEntryPoint> Fixture { get; }

    public async Task InitializeAsync()
    {

        MongoConnectionString = Fixture.MongoConnectionString;
        DefaultConnectionString = Fixture.SqlConnectionString;
        PersistConnectionString = Fixture.SqlPersistConnectionString;

        await SeedDataAsync();
    }

    public async Task DisposeAsync()
    {
        await Task.CompletedTask;
    }

    protected virtual void RegisterTestsServices(IServiceCollection services)
    {
    }

    private async Task SeedDataAsync()
    {
        using var scope = Fixture.ServiceProvider.CreateScope();

        var seeders = scope.ServiceProvider.GetServices<IDataSeeder>();
        foreach (var seeder in seeders) await seeder.SeedAllAsync();
    }
}

public abstract class IntegrationTestBase<TEntryPoint> : IntegrationTestFixtureCore<TEntryPoint>,
    IClassFixture<IntegrationTestFactory<TEntryPoint>>
    where TEntryPoint : class
{
    protected IntegrationTestBase(
        IntegrationTestFactory<TEntryPoint> integrationTestFixture) : base(integrationTestFixture)
    {
        Fixture = integrationTestFixture;
    }

    public new IntegrationTestFactory<TEntryPoint> Fixture { get; }
}

public abstract class IntegrationTestBase<TEntryPoint, TWContext> : IntegrationTestFixtureCore<TEntryPoint>,
    IClassFixture<IntegrationTestFactory<TEntryPoint, TWContext>>
    where TEntryPoint : class
    where TWContext : DbContext
{
    protected IntegrationTestBase(
        IntegrationTestFactory<TEntryPoint, TWContext> integrationTestFixture) : base(integrationTestFixture)
    {
        Fixture = integrationTestFixture;
    }

    public new IntegrationTestFactory<TEntryPoint, TWContext> Fixture { get; }
}

public abstract class IntegrationTestBase<TEntryPoint, TWContext, TRContext> : IntegrationTestFixtureCore<TEntryPoint>,
    IClassFixture<IntegrationTestFactory<TEntryPoint, TWContext, TRContext>>
    where TEntryPoint : class
    where TWContext : DbContext
    where TRContext : MongoDbContext
{
    protected IntegrationTestBase(
        IntegrationTestFactory<TEntryPoint, TWContext, TRContext> integrationTestFixture) : base(integrationTestFixture)
    {
        Fixture = integrationTestFixture;
    }

    public new IntegrationTestFactory<TEntryPoint, TWContext, TRContext> Fixture { get; }
}

public abstract class IntegrationTestBase<TEntryPoint, TWContext, TRContext, PContext> :
    IntegrationTestFixtureCore<TEntryPoint>,
    IClassFixture<IntegrationTestFactory<TEntryPoint, TWContext, TRContext, PContext>>
    where TEntryPoint : class
    where TWContext : DbContext
    where TRContext : MongoDbContext
    where PContext : PersistMessageDbContext
{
    protected IntegrationTestBase(
        IntegrationTestFactory<TEntryPoint, TWContext, TRContext, PContext> integrationTestFixture) : base(
        integrationTestFixture)
    {
        Fixture = integrationTestFixture;
    }

    public new IntegrationTestFactory<TEntryPoint, TWContext, TRContext, PContext> Fixture { get; }
}
