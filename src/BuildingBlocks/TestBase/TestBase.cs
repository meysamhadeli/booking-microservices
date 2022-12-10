using Ardalis.GuardClauses;
using BuildingBlocks.Core.Event;
using BuildingBlocks.Core.Model;
using BuildingBlocks.EFCore;
using BuildingBlocks.MassTransit;
using BuildingBlocks.Mongo;
using BuildingBlocks.PersistMessageProcessor;
using BuildingBlocks.TestBase.Auth;
using BuildingBlocks.Web;
using DotNet.Testcontainers.Containers;
using EasyNetQ.Management.Client;
using Grpc.Net.Client;
using MassTransit;
using MassTransit.Testing;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using NSubstitute;
using Respawn;
using Respawn.Graph;
using Serilog;
using Xunit;
using Xunit.Abstractions;
using ILogger = Serilog.ILogger;

namespace BuildingBlocks.TestBase;

public class TestFactory<TEntryPoint> : IAsyncLifetime
    where TEntryPoint : class
{
    private readonly WebApplicationFactory<TEntryPoint> _factory;
    private int Timeout => 120; // Second

    public MsSqlTestcontainer MsSqlTestContainer;
    public MsSqlTestcontainer MsSqlPersistTestContainer;
    public RabbitMqTestcontainer RabbitMqTestContainer;
    public MongoDbTestcontainer MongoDbTestContainer;

    private ITestHarness TestHarness => ServiceProvider?.GetTestHarness();
    public HttpClient HttpClient => _factory?.CreateClient();
    public GrpcChannel Channel => GrpcChannel.ForAddress(HttpClient.BaseAddress!, new GrpcChannelOptions { HttpClient = HttpClient });
    public Action<IServiceCollection> TestRegistrationServices { get; set; }
    public IServiceProvider ServiceProvider => _factory?.Services;
    public IConfiguration Configuration => _factory?.Services.GetRequiredService<IConfiguration>();
    public ILogger Logger { get; set; }

    public TestFactory()
    {
        _factory = new WebApplicationFactory<TEntryPoint>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureAppConfiguration(AddCustomAppSettings);

                builder.UseEnvironment("test");
                builder.ConfigureServices(services =>
                {
                    TestRegistrationServices?.Invoke(services);
                    services.ReplaceSingleton(AddHttpContextAccessorMock);
                    services.AddTestAuthentication();
                });
            });
    }

    public async Task InitializeAsync()
    {
        await StartTestContainerAsync();
    }

    public async Task DisposeAsync()
    {
        await StopTestContainerAsync();
        _factory?.DisposeAsync();
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

    public async Task Publish<TMessage>(TMessage message, CancellationToken cancellationToken = default)
        where TMessage : class, IEvent
    {
        await TestHarness.Bus.Publish<TMessage>(message, cancellationToken);
    }

    public async Task<bool> WaitForPublishing<TMessage>(CancellationToken cancellationToken = default)
        where TMessage : class, IEvent
    {
        var result = await WaitUntilConditionMet(async () =>
        {
            var published = await TestHarness.Published.Any<TMessage>(cancellationToken);
            var faulty = await TestHarness.Published.Any<Fault<TMessage>>(cancellationToken);

            return published && faulty == false;
        });
        return result;
    }

    public async Task<bool> WaitForConsuming<TMessage>(CancellationToken cancellationToken = default)
        where TMessage : class, IEvent
    {
        var result = await WaitUntilConditionMet(async () =>
        {
            var consumed = await TestHarness.Consumed.Any<TMessage>(cancellationToken);
            var faulty = await TestHarness.Consumed.Any<Fault<TMessage>>(cancellationToken);

            return consumed && faulty == false;
        });

        return result;
    }

    // Ref: https://tech.energyhelpline.com/in-memory-testing-with-masstransit/
    public async Task<bool> WaitUntilConditionMet(Func<Task<bool>> conditionToMet, int? timeoutSecond = null)
    {
        var time = timeoutSecond ?? Timeout;

        var startTime = DateTime.Now;
        var timeoutExpired = false;
        var meet = await conditionToMet.Invoke();
        while (!meet)
        {
            if (timeoutExpired) return false;

            await Task.Delay(100);
            meet = await conditionToMet.Invoke();
            timeoutExpired = DateTime.Now - startTime > TimeSpan.FromSeconds(time);
        }

        return true;
    }

    public async Task<bool> ShouldProcessedPersistInternalCommand<TInternalCommand>()
        where TInternalCommand : class, IInternalCommand
    {
        var result = await WaitUntilConditionMet(async () =>
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

        return result;
    }


    private async Task StartTestContainerAsync()
    {
        MsSqlTestContainer = TestContainers.MsSqlTestContainer;
        MsSqlPersistTestContainer = TestContainers.MsSqlPersistTestContainer;
        RabbitMqTestContainer = TestContainers.RabbitMqTestContainer;
        MongoDbTestContainer = TestContainers.MongoTestContainer;

        await MongoDbTestContainer.StartAsync();
        await MsSqlTestContainer.StartAsync();
        await MsSqlPersistTestContainer.StartAsync();
        await RabbitMqTestContainer.StartAsync();
    }

    private async Task StopTestContainerAsync()
    {
        await MsSqlTestContainer.StopAsync();
        await MsSqlPersistTestContainer.StopAsync();
        await RabbitMqTestContainer.StopAsync();
        await MongoDbTestContainer.StopAsync();
    }

    private void AddCustomAppSettings(IConfigurationBuilder configuration)
    {
        configuration.AddInMemoryCollection(new KeyValuePair<string, string>[]
        {
            new("DatabaseOptions:DefaultConnection", MsSqlTestContainer.ConnectionString + "TrustServerCertificate=True"),
            new("PersistMessageOptions:ConnectionString", MsSqlPersistTestContainer.ConnectionString + "TrustServerCertificate=True"),
            new("RabbitMqOptions:HostName", RabbitMqTestContainer.Hostname),
            new("RabbitMqOptions:UserName", RabbitMqTestContainer.Username),
            new("RabbitMqOptions:Password", RabbitMqTestContainer.Password),
            new("RabbitMqOptions:Port", RabbitMqTestContainer.Port.ToString()),
            new("MongoOptions:ConnectionString", MongoDbTestContainer.ConnectionString),
            new("MongoOptions:DatabaseName", MongoDbTestContainer.Database)
        });
    }

    private IHttpContextAccessor AddHttpContextAccessorMock(IServiceProvider serviceProvider)
    {
        var httpContextAccessorMock = Substitute.For<IHttpContextAccessor>();
        using var scope = serviceProvider.CreateScope();
        httpContextAccessorMock.HttpContext = new DefaultHttpContext { RequestServices = scope.ServiceProvider };

        httpContextAccessorMock.HttpContext.Request.Host = new HostString("localhost", 6012);
        httpContextAccessorMock.HttpContext.Request.Scheme = "http";

        return httpContextAccessorMock;
    }
}

public class TestFactory<TEntryPoint, TWContext> : TestFactory<TEntryPoint>
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

public class TestFactory<TEntryPoint, TWContext, TRContext> : TestFactory<TEntryPoint, TWContext>
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

public class TestFixtureCore<TEntryPoint> : IAsyncLifetime
    where TEntryPoint : class
{
    private Respawner _reSpawnerDefaultDb;
    private Respawner _reSpawnerPersistDb;
    private SqlConnection DefaultDbConnection { get; set; }
    private SqlConnection PersistDbConnection { get; set; }

    public TestFixtureCore(TestFactory<TEntryPoint> integrationTestFixture, ITestOutputHelper outputHelper)
    {
        Fixture = integrationTestFixture;
        integrationTestFixture.RegisterServices(services => RegisterTestsServices(services));
        integrationTestFixture.Logger = integrationTestFixture.CreateLogger(outputHelper);
    }

    public TestFactory<TEntryPoint> Fixture { get; }


    public async Task InitializeAsync()
    {
        var databaseOptions = Fixture.ServiceProvider.GetRequiredService<IOptions<DatabaseOptions>>()?.Value;
        var persistOptions = Fixture.ServiceProvider.GetRequiredService<IOptions<PersistMessageOptions>>()?.Value;

        if (!string.IsNullOrEmpty(persistOptions?.ConnectionString))
        {
            PersistDbConnection = new SqlConnection(persistOptions?.ConnectionString);
            await PersistDbConnection.OpenAsync();

            _reSpawnerPersistDb = await Respawner.CreateAsync(PersistDbConnection,
                new RespawnerOptions { TablesToIgnore = new Table[] { "__EFMigrationsHistory" }, });

            await _reSpawnerPersistDb.ResetAsync(PersistDbConnection);
        }

        if (!string.IsNullOrEmpty(databaseOptions?.DefaultConnection))
        {
            DefaultDbConnection = new SqlConnection(databaseOptions.DefaultConnection);
            await DefaultDbConnection.OpenAsync();

            _reSpawnerDefaultDb = await Respawner.CreateAsync(DefaultDbConnection,
                new RespawnerOptions { TablesToIgnore = new Table[] { "__EFMigrationsHistory" }, });

            await _reSpawnerDefaultDb.ResetAsync(DefaultDbConnection);

            await SeedDataAsync();
        }
    }

    public async Task DisposeAsync()
    {
        await ResetMongoAsync();
        await ResetRabbitMqAsync();
    }

    private async Task ResetMongoAsync(CancellationToken cancellationToken = default)
    {
        //https://stackoverflow.com/questions/3366397/delete-everything-in-a-mongodb-database
        MongoClient dbClient = new MongoClient(Fixture.MongoDbTestContainer?.ConnectionString);
        var collections = await dbClient.GetDatabase(Fixture.MongoDbTestContainer?.Database)
            .ListCollectionsAsync(cancellationToken: cancellationToken);

        foreach (var collection in collections.ToList())
        {
            await dbClient.GetDatabase(Fixture.MongoDbTestContainer?.Database)
                .DropCollectionAsync(collection["name"].AsString, cancellationToken);
        }
    }

    private async Task ResetRabbitMqAsync(CancellationToken cancellationToken = default)
    {
        var port = Fixture.RabbitMqTestContainer?.GetMappedPublicPort(15672) ?? 15672;

        var rabbitmqOptions = Fixture.ServiceProvider.GetRequiredService<IOptions<RabbitMqOptions>>()?.Value;

        var managementClient = new ManagementClient(rabbitmqOptions?.HostName, rabbitmqOptions?.UserName,
            rabbitmqOptions?.Password, port);

        var bd = await managementClient.GetBindingsAsync(cancellationToken);
        var bindings = bd.Where(x => !string.IsNullOrEmpty(x.Source) && !string.IsNullOrEmpty(x.Destination));

        foreach (var binding in bindings)
        {
            await managementClient.DeleteBindingAsync(binding, cancellationToken);
        }

        var queues = await managementClient.GetQueuesAsync(cancellationToken);

        foreach (var queue in queues)
        {
            await managementClient.DeleteQueueAsync(queue, cancellationToken);
        }
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

public abstract class TestBase<TEntryPoint, TWContext> : TestFixtureCore<TEntryPoint>
    //,IClassFixture<IntegrationTestFactory<TEntryPoint, TWContext>>
    where TEntryPoint : class
    where TWContext : DbContext
{
    protected TestBase(
        TestFactory<TEntryPoint, TWContext> integrationTestFixture, ITestOutputHelper outputHelper = null) : base(integrationTestFixture, outputHelper)
    {
        Fixture = integrationTestFixture;
    }

    public TestFactory<TEntryPoint, TWContext> Fixture { get; }
}

public abstract class TestBase<TEntryPoint, TWContext, TRContext> : TestFixtureCore<TEntryPoint>
    //,IClassFixture<IntegrationTestFactory<TEntryPoint, TWContext, TRContext>>
    where TEntryPoint : class
    where TWContext : DbContext
    where TRContext : MongoDbContext
{
    protected TestBase(
        TestFactory<TEntryPoint, TWContext, TRContext> integrationTestFixture, ITestOutputHelper outputHelper = null) : base(integrationTestFixture, outputHelper)
    {
        Fixture = integrationTestFixture;
    }

    public TestFactory<TEntryPoint, TWContext, TRContext> Fixture { get; }
}
