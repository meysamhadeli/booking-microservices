using Ardalis.GuardClauses;
using BuildingBlocks.Core.Event;
using BuildingBlocks.Core.Model;
using BuildingBlocks.EFCore;
using BuildingBlocks.Mongo;
using BuildingBlocks.PersistMessageProcessor;
using DotNet.Testcontainers.Containers;
using Grpc.Net.Client;
using MassTransit.Testing;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Mongo2Go;
using Respawn;
using Serilog;
using Xunit;
using Xunit.Abstractions;

namespace BuildingBlocks.TestBase;

public class IntegrationTestFixture<TEntryPoint> : IDisposable
    where TEntryPoint : class
{
    private readonly CustomWebApplicationFactory<TEntryPoint> _factory;

    private int Timeout => 180;
    public HttpClient HttpClient => _factory.CreateClient();
    public ITestHarness TestHarness => ServiceProvider.GetTestHarness();

    public GrpcChannel Channel =>
        GrpcChannel.ForAddress(HttpClient.BaseAddress!, new GrpcChannelOptions {HttpClient = HttpClient});

    public IServiceProvider ServiceProvider => _factory.Services;
    public IConfiguration Configuration => _factory.Services.GetRequiredService<IConfiguration>();

    public MsSqlTestcontainer SqlTestContainer;
    public MsSqlTestcontainer SqlPersistTestContainer;
    public MongoDbTestcontainer MongoTestContainer;

    public IntegrationTestFixture()
    {
        // Ref: https://docs.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-6.0#basic-tests-with-the-default-webapplicationfactory
        _factory = new CustomWebApplicationFactory<TEntryPoint>();
    }

    public void Dispose()
    {
        _factory.Dispose();
    }

    public virtual void RegisterServices(Action<IServiceCollection> services)
    {
        _factory.TestRegistrationServices = services;
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
}

public class IntegrationTestFixture<TEntryPoint, TWContext> : IntegrationTestFixture<TEntryPoint>
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

public class IntegrationTestFixture<TEntryPoint, TWContext, TRContext> : IntegrationTestFixture<TEntryPoint, TWContext>
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
    private Checkpoint _checkpointDefaultDB;
    private Checkpoint _checkpointPersistMessageDB;
    private MongoDbRunner _mongoRunner;

    private string DefaultConnectionString
    {
        get => Fixture.ServiceProvider.GetRequiredService<IOptions<ConnectionStrings>>()?.Value.DefaultConnection;
        set => Fixture.ServiceProvider.GetRequiredService<IOptions<ConnectionStrings>>().Value.DefaultConnection =
            value;
    }

    private string PersistConnectionString
    {
        get => Fixture.ServiceProvider.GetRequiredService<IOptions<PersistMessageOptions>>()?.Value.ConnectionString;
        set => Fixture.ServiceProvider.GetRequiredService<IOptions<PersistMessageOptions>>().Value.ConnectionString =
            value;
    }

    private string MongoConnectionString
    {
        get => Fixture.ServiceProvider.GetRequiredService<IOptions<MongoOptions>>()?.Value?.ConnectionString;
        set => Fixture.ServiceProvider.GetRequiredService<IOptions<MongoOptions>>().Value.ConnectionString = value;
    }

    public IntegrationTestFixtureCore(IntegrationTestFixture<TEntryPoint> integrationTestFixture)
    {
        Fixture = integrationTestFixture;
        integrationTestFixture.RegisterServices(services => RegisterTestsServices(services));
    }

    public IntegrationTestFixture<TEntryPoint> Fixture { get; }

    public async Task InitializeAsync()
    {
        _checkpointDefaultDB = new Checkpoint {TablesToIgnore = new[] {"__EFMigrationsHistory"}};
        _checkpointPersistMessageDB = new Checkpoint {TablesToIgnore = new[] {"__EFMigrationsHistory"}};

        _mongoRunner = MongoDbRunner.Start();

        if (MongoConnectionString != null)
            MongoConnectionString = _mongoRunner.ConnectionString;

        // <<For using test-container base>>
        // Fixture.SqlTestContainer = TestContainers.SqlTestContainer;
        // Fixture.SqlPersistTestContainer = TestContainers.SqlPersistTestContainer;
        // Fixture.MongoTestContainer = TestContainers.MongoTestContainer;
        //
        // await Fixture.SqlTestContainer.StartAsync();
        // await Fixture.SqlPersistTestContainer.StartAsync();
        // await Fixture.MongoTestContainer.StartAsync();
        //
        // DefaultConnectionString = Fixture.SqlTestContainer?.ConnectionString;
        // PersistConnectionString = Fixture.SqlPersistTestContainer?.ConnectionString;
        // MongoConnectionString = Fixture.MongoTestContainer?.ConnectionString;

        await SeedDataAsync();
    }

    public async Task DisposeAsync()
    {
        if (!string.IsNullOrEmpty(DefaultConnectionString))
            await _checkpointDefaultDB.Reset(DefaultConnectionString);

        if (!string.IsNullOrEmpty(PersistConnectionString))
            await _checkpointPersistMessageDB.Reset(PersistConnectionString);

        if (!string.IsNullOrEmpty(PersistConnectionString))
            _mongoRunner.Dispose();

        // <<For using test-container base>>
        // await Fixture.SqlTestContainer.StopAsync();
        // await Fixture.SqlPersistTestContainer.StopAsync();
        // await Fixture.MongoTestContainer.StopAsync();
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
    IClassFixture<IntegrationTestFixture<TEntryPoint>>
    where TEntryPoint : class
{
    protected IntegrationTestBase(
        IntegrationTestFixture<TEntryPoint> integrationTestFixture) : base(integrationTestFixture)
    {
        Fixture = integrationTestFixture;
    }

    public IntegrationTestFixture<TEntryPoint> Fixture { get; }
}

public abstract class IntegrationTestBase<TEntryPoint, TWContext> : IntegrationTestFixtureCore<TEntryPoint>,
    IClassFixture<IntegrationTestFixture<TEntryPoint, TWContext>>
    where TEntryPoint : class
    where TWContext : DbContext
{
    protected IntegrationTestBase(
        IntegrationTestFixture<TEntryPoint, TWContext> integrationTestFixture) : base(integrationTestFixture)
    {
        Fixture = integrationTestFixture;
    }

    public IntegrationTestFixture<TEntryPoint, TWContext> Fixture { get; }
}

public abstract class IntegrationTestBase<TEntryPoint, TWContext, TRContext> : IntegrationTestFixtureCore<TEntryPoint>,
    IClassFixture<IntegrationTestFixture<TEntryPoint, TWContext, TRContext>>
    where TEntryPoint : class
    where TWContext : DbContext
    where TRContext : MongoDbContext
{
    protected IntegrationTestBase(
        IntegrationTestFixture<TEntryPoint, TWContext, TRContext> integrationTestFixture) : base(integrationTestFixture)
    {
        Fixture = integrationTestFixture;
    }

    public IntegrationTestFixture<TEntryPoint, TWContext, TRContext> Fixture { get; }
}
