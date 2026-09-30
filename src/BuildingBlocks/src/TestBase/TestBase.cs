using System.Net;
using System.Security.Claims;
using Woo.Core.Event;
using Woo.Core.Model;
using Woo.EFCore;
using Woo.Mongo;
using Woo.Web;
using Duende.IdentityServer.EntityFramework.Entities;
using EasyNetQ.Management.Client;
using Grpc.Net.Client;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using NSubstitute;
using Respawn;
using WebMotions.Fake.Authentication.JwtBearer;
using Xunit;
using Xunit.Abstractions;

namespace Woo.TestBase;

using System.Globalization;
using Npgsql;
using Testcontainers.EventStoreDb;
using Testcontainers.MongoDb;
using Testcontainers.PostgreSql;
using Testcontainers.RabbitMq;
using global::Wolverine;
using global::Wolverine.Tracking;

public class TestFixture<TEntryPoint> : IAsyncLifetime
    where TEntryPoint : class
{
    private readonly WebApplicationFactory<TEntryPoint> _factory;
    private readonly SemaphoreSlim _initializationLock = new(1, 1);
    private readonly Dictionary<string, string?> _originalEnvironmentVariables = new(StringComparer.OrdinalIgnoreCase);
    private ITrackedSession _lastTrackedSession;
    private Dictionary<string, int> _outgoingEnvelopeCountsBeforeLastAction = new(StringComparer.Ordinal);
    private bool _initialized;
    private int Timeout => 120; // Second
    private Action<IServiceCollection> TestRegistrationServices { get; set; }
    private PostgreSqlContainer PostgresTestcontainer;
    public RabbitMqContainer RabbitMqTestContainer;
    public MongoDbContainer MongoDbTestContainer;
    public EventStoreDbContainer EventStoreDbTestContainer;
    public CancellationTokenSource CancellationTokenSource;

    public HttpClient HttpClient
    {
        get
        {
            var claims = new Dictionary<string, object>
            {
                { ClaimTypes.Name, "test@sample.com" },
                { ClaimTypes.Role, "admin" },
                { "scope", "flight-api" },
            };

            var httpClient = _factory.CreateClient();
            httpClient.SetFakeBearerToken(claims); // Uses FakeJwtBearer
            return httpClient;
        }
    }

    public GrpcChannel Channel =>
        GrpcChannel.ForAddress(HttpClient.BaseAddress!, new GrpcChannelOptions { HttpClient = HttpClient });

    public IServiceProvider ServiceProvider => _factory?.Services;
    public IConfiguration Configuration => _factory?.Services.GetRequiredService<IConfiguration>();
    public ILogger Logger { get; set; }

    internal string GetPostgresConnectionString() => PostgresTestcontainer.GetConnectionString();

    protected TestFixture()
    {
        _factory = new WebApplicationFactory<TEntryPoint>().WithWebHostBuilder(builder =>
        {
            builder.ConfigureAppConfiguration(AddCustomAppSettings);

            builder.UseEnvironment("test");

            builder.ConfigureServices(services =>
            {
                TestRegistrationServices?.Invoke(services);
                services.ReplaceSingleton(AddHttpContextAccessorMock);

                // Register all ITestDataSeeder implementations dynamically
                services.Scan(scan =>
                    scan.FromApplicationDependencies() // Scan the current app and its dependencies
                        .AddClasses(classes => classes.AssignableTo<ITestDataSeeder>()) // Find classes that implement ITestDataSeeder
                        .AsImplementedInterfaces()
                        .WithScopedLifetime()
                );

                // Add Fake JWT Authentication - we can use SetAdminUser method to set authenticate user to existing HttContextAccessor
                // https://github.com/webmotions/fake-authentication-jwtbearer
                // https://github.com/webmotions/fake-authentication-jwtbearer/issues/14
                services
                    .AddAuthentication(options =>
                    {
                        options.DefaultAuthenticateScheme = FakeJwtBearerDefaults.AuthenticationScheme;

                        options.DefaultChallengeScheme = FakeJwtBearerDefaults.AuthenticationScheme;
                    })
                    .AddFakeJwtBearer();

                // Mock Authorization Policies
                services.AddAuthorization(options =>
                {
                    options.AddPolicy(
                        nameof(ApiScope),
                        policy =>
                        {
                            policy.AddAuthenticationSchemes(FakeJwtBearerDefaults.AuthenticationScheme);
                            policy.RequireAuthenticatedUser();
                            policy.RequireClaim("scope", "flight-api"); // Test-specific scope
                        }
                    );
                });
            });
        });
    }

    public async Task InitializeAsync()
    {
        await EnsureInitializedAsync();
    }

    public async Task DisposeAsync()
    {
        if (!_initialized)
            return;

        await StopTestContainerAsync();
        await _factory.DisposeAsync();
        await CancellationTokenSource.CancelAsync();
        RestoreEnvironmentVariables();
    }

    internal async Task EnsureInitializedAsync()
    {
        if (_initialized)
            return;

        await _initializationLock.WaitAsync();

        try
        {
            if (_initialized)
                return;

            CancellationTokenSource = new CancellationTokenSource();
            await StartTestContainerAsync();
            ApplyTestEnvironmentVariables();
            await EnsureTestDatabasesExistAsync();
            _initialized = true;
        }
        finally
        {
            _initializationLock.Release();
        }
    }

    public virtual void RegisterServices(Action<IServiceCollection> services)
    {
        TestRegistrationServices += services;
    }

    // ref: https://github.com/trbenning/serilog-sinks-xunit
    public ILogger CreateLogger(ITestOutputHelper output)
    {
        if (output == null)
            return null;

        var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddXunit(output);
            builder.SetMinimumLevel(LogLevel.Debug);
        });
        return loggerFactory.CreateLogger("TestLogger");
    }

    protected async Task ExecuteScopeAsync(Func<IServiceProvider, Task> action)
    {
        using var scope = ServiceProvider.CreateScope();
        await action(scope.ServiceProvider);
    }

    protected async Task<T> ExecuteScopeAsync<T>(Func<IServiceProvider, Task<T>> action)
    {
        using var scope = ServiceProvider.CreateScope();

        var result = await action(scope.ServiceProvider);

        return result;
    }

    public Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request)
    {
        return ExecuteScopeAsync(async sp =>
        {
            var mediator = sp.GetRequiredService<IMediator>();
            await CaptureOutgoingEnvelopeCountsAsync();

            TResponse response = default;
            _lastTrackedSession = await sp.TrackActivity()
                .Timeout(TimeSpan.FromSeconds(Timeout))
                .ExecuteAndWaitAsync((Func<IMessageContext, Task>)(async _ => { response = await mediator.Send(request); }));

            return response;
        });
    }

    public Task SendAsync(IRequest request)
    {
        return ExecuteScopeAsync(async sp =>
        {
            var mediator = sp.GetRequiredService<IMediator>();
            await CaptureOutgoingEnvelopeCountsAsync();
            _lastTrackedSession = await sp.TrackActivity()
                .Timeout(TimeSpan.FromSeconds(Timeout))
                .ExecuteAndWaitAsync((Func<IMessageContext, Task>)(async _ => { await mediator.Send(request); }));
        });
    }

    public async Task Publish<TMessage>(TMessage message, CancellationToken cancellationToken = default)
        where TMessage : class, IEvent
    {
        await ExecuteScopeAsync(async sp =>
        {
            var bus = sp.GetRequiredService<IMessageBus>();
            await CaptureOutgoingEnvelopeCountsAsync();
            _lastTrackedSession = await sp.TrackActivity()
                .Timeout(TimeSpan.FromSeconds(Timeout))
                .ExecuteAndWaitAsync((Func<IMessageContext, Task>)(async _ =>
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    await bus.PublishAsync(message);
                }));
        });
    }

    public async Task<bool> WaitForPublishing<TMessage>(CancellationToken cancellationToken = default)
        where TMessage : class, IEvent
    {
        cancellationToken.ThrowIfCancellationRequested();
        var baselineCount = GetTrackedMessageCount(typeof(TMessage), _outgoingEnvelopeCountsBeforeLastAction);

        return await WaitUntilAsync(async () =>
        {
            var currentCounts = await ReadOutgoingEnvelopeCountsAsync(cancellationToken);
            if (GetTrackedMessageCount(typeof(TMessage), currentCounts) > baselineCount)
            {
                return true;
            }

            return _lastTrackedSession is not null
                   && _lastTrackedSession.Sent.AllMessages().OfType<TMessage>().Any();
        }, cancellationToken: cancellationToken);
    }

    private async Task CaptureOutgoingEnvelopeCountsAsync(CancellationToken cancellationToken = default)
    {
        _outgoingEnvelopeCountsBeforeLastAction = await ReadOutgoingEnvelopeCountsAsync(cancellationToken);
    }

    private async Task<Dictionary<string, int>> ReadOutgoingEnvelopeCountsAsync(CancellationToken cancellationToken = default)
    {
        if (PostgresTestcontainer is null)
        {
            return new Dictionary<string, int>(StringComparer.Ordinal);
        }

        try
        {
            await using var connection = new NpgsqlConnection(PostgresTestcontainer.GetConnectionString());
            await connection.OpenAsync(cancellationToken);

            const string sql = """
                               select message_type, count(*)
                               from wolverine.wolverine_outgoing_envelopes
                               group by message_type
                               """;

            await using var command = new NpgsqlCommand(sql, connection);
            await using var reader = await command.ExecuteReaderAsync(cancellationToken);

            var counts = new Dictionary<string, int>(StringComparer.Ordinal);
            while (await reader.ReadAsync(cancellationToken))
            {
                counts[reader.GetString(0)] = reader.GetInt32(1);
            }

            return counts;
        }
        catch (PostgresException ex) when (ex.SqlState == PostgresErrorCodes.InvalidSchemaName || ex.SqlState == PostgresErrorCodes.UndefinedTable)
        {
            return new Dictionary<string, int>(StringComparer.Ordinal);
        }
    }

    private static int GetTrackedMessageCount(Type messageType, IReadOnlyDictionary<string, int> counts)
    {
        var fullName = messageType.FullName;
        var shortName = messageType.Name;

        return counts
            .Where(pair => pair.Key.Contains(fullName ?? shortName, StringComparison.Ordinal)
                           || pair.Key.EndsWith(shortName, StringComparison.Ordinal))
            .Sum(pair => pair.Value);
    }

    public Task<bool> WaitUntilAsync(
        Func<Task<bool>> condition,
        TimeSpan? timeout = null,
        TimeSpan? pollInterval = null,
        CancellationToken cancellationToken = default
    )
    {
        var effectiveTimeout = timeout ?? TimeSpan.FromSeconds(Timeout);
        var effectivePollInterval = pollInterval ?? TimeSpan.FromMilliseconds(200);

        return WaitUntilConditionMet(
            conditionToMet: async () =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                return await condition();
            },
            timeoutSecond: (int)Math.Ceiling(effectiveTimeout.TotalSeconds),
            pollInterval: effectivePollInterval,
            cancellationToken: cancellationToken
        );
    }

    // Ref: https://tech.energyhelpline.com/in-memory-testing-with-message-brokers/
    private async Task<bool> WaitUntilConditionMet(
        Func<Task<bool>> conditionToMet,
        int? timeoutSecond = null,
        TimeSpan? pollInterval = null,
        CancellationToken cancellationToken = default
    )
    {
        var time = timeoutSecond ?? Timeout;
        var delay = pollInterval ?? TimeSpan.FromMilliseconds(100);

        var startTime = DateTime.UtcNow;
        while (DateTime.UtcNow - startTime <= TimeSpan.FromSeconds(time))
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (await conditionToMet.Invoke())
                return true;

            await Task.Delay(delay, cancellationToken);
        }

        return false;
    }

    private async Task StartTestContainerAsync()
    {
        PostgresTestcontainer = TestContainers.PostgresTestContainer();
        RabbitMqTestContainer = TestContainers.RabbitMqTestContainer();
        MongoDbTestContainer = TestContainers.MongoTestContainer();
        EventStoreDbTestContainer = TestContainers.EventStoreTestContainer();

        await MongoDbTestContainer.StartAsync();
        await PostgresTestcontainer.StartAsync();
        await RabbitMqTestContainer.StartAsync();
        await EventStoreDbTestContainer.StartAsync();
    }

    private async Task StopTestContainerAsync()
    {
        if (PostgresTestcontainer is not null)
            await PostgresTestcontainer.StopAsync();

        if (RabbitMqTestContainer is not null)
            await RabbitMqTestContainer.StopAsync();

        if (MongoDbTestContainer is not null)
            await MongoDbTestContainer.StopAsync();

        if (EventStoreDbTestContainer is not null)
            await EventStoreDbTestContainer.StopAsync();
    }

    private async Task EnsureTestDatabasesExistAsync()
    {
        await EnsureDatabasesExistAsync(
            PostgresTestcontainer.GetConnectionString(),
            ["booking_test", "flight_test", "identity_test", "passenger_test"]);
    }

    private static async Task EnsureDatabasesExistAsync(string connectionString, IEnumerable<string> databaseNames)
    {
        var connectionStringBuilder = new NpgsqlConnectionStringBuilder(connectionString)
        {
            Database = "postgres"
        };

        await using var connection = new NpgsqlConnection(connectionStringBuilder.ConnectionString);
        await connection.OpenAsync();

        foreach (var databaseName in databaseNames.Distinct(StringComparer.OrdinalIgnoreCase))
        {
            await using var command = connection.CreateCommand();
            command.CommandText =
                $"CREATE DATABASE \"{databaseName.Replace("\"", "\"\"", StringComparison.Ordinal)}\"";

            try
            {
                await command.ExecuteNonQueryAsync();
            }
            catch (PostgresException exception) when (exception.SqlState == PostgresErrorCodes.DuplicateDatabase)
            {
            }
        }
    }

    private void ApplyTestEnvironmentVariables()
    {
        var postgresConnectionString = PostgresTestcontainer.GetConnectionString();

        SetEnvironmentVariable("ConnectionStrings__postgres", postgresConnectionString);
        SetEnvironmentVariable("ConnectionStrings__wolverine", postgresConnectionString);
        SetEnvironmentVariable("ConnectionStrings__booking", postgresConnectionString);
        SetEnvironmentVariable("ConnectionStrings__flight", postgresConnectionString);
        SetEnvironmentVariable("ConnectionStrings__identity", postgresConnectionString);
        SetEnvironmentVariable("ConnectionStrings__passenger", postgresConnectionString);
        SetEnvironmentVariable("PostgresOptions__ConnectionString", postgresConnectionString);
        SetEnvironmentVariable("PostgresOptions__ConnectionString__Booking", postgresConnectionString);
        SetEnvironmentVariable("PostgresOptions__ConnectionString__Flight", postgresConnectionString);
        SetEnvironmentVariable("PostgresOptions__ConnectionString__Identity", postgresConnectionString);
        SetEnvironmentVariable("PostgresOptions__ConnectionString__Passenger", postgresConnectionString);
    }

    private void SetEnvironmentVariable(string key, string value)
    {
        if (!_originalEnvironmentVariables.ContainsKey(key))
        {
            _originalEnvironmentVariables[key] = Environment.GetEnvironmentVariable(key);
        }

        Environment.SetEnvironmentVariable(key, value);
    }

    private void RestoreEnvironmentVariables()
    {
        foreach (var pair in _originalEnvironmentVariables)
        {
            Environment.SetEnvironmentVariable(pair.Key, pair.Value);
        }

        _originalEnvironmentVariables.Clear();
    }

    private void AddCustomAppSettings(IConfigurationBuilder configuration)
    {
        var postgresConnectionString = PostgresTestcontainer.GetConnectionString();

        //todo: provide better approach for reading `PostgresOptions`
        configuration.AddInMemoryCollection(
            new KeyValuePair<string, string>[]
            {
                new("ConnectionStrings:postgres", postgresConnectionString),
                new("ConnectionStrings:wolverine", postgresConnectionString),
                new("ConnectionStrings:booking", postgresConnectionString),
                new("ConnectionStrings:flight", postgresConnectionString),
                new("ConnectionStrings:identity", postgresConnectionString),
                new("ConnectionStrings:passenger", postgresConnectionString),
                new("PostgresOptions:ConnectionString", postgresConnectionString),
                new("PostgresOptions:ConnectionString:Booking", postgresConnectionString),
                new("PostgresOptions:ConnectionString:Flight", postgresConnectionString),
                new("PostgresOptions:ConnectionString:Identity", postgresConnectionString),
                new("PostgresOptions:ConnectionString:Passenger", postgresConnectionString),
                new("RabbitMqOptions:HostName", "127.0.0.1"),
                new("RabbitMqOptions:UserName", TestContainers.RabbitMqContainerConfiguration.UserName),
                new("RabbitMqOptions:Password", TestContainers.RabbitMqContainerConfiguration.Password),
                new(
                    "RabbitMqOptions:Port",
                    RabbitMqTestContainer
                        .GetMappedPublicPort(TestContainers.RabbitMqContainerConfiguration.Port)
                        .ToString(NumberFormatInfo.InvariantInfo)
                ),
                new("MongoOptions:ConnectionString", MongoDbTestContainer.GetConnectionString()),
                new("MongoOptions:DatabaseName", TestContainers.MongoContainerConfiguration.Name),
                new("EventStoreOptions:ConnectionString", EventStoreDbTestContainer.GetConnectionString()),
            }
        );
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

public class TestWriteFixture<TEntryPoint, TWContext> : TestFixture<TEntryPoint>
    where TEntryPoint : class
    where TWContext : DbContext
{
    public Task ExecuteDbContextAsync(Func<TWContext, Task> action)
    {
        return ExecuteScopeAsync(sp => action(sp.GetRequiredService<TWContext>()));
    }

    public Task ExecuteDbContextAsync(Func<TWContext, ValueTask> action)
    {
        return ExecuteScopeAsync(sp => action(sp.GetRequiredService<TWContext>()).AsTask());
    }

    public Task ExecuteDbContextAsync(Func<TWContext, IMediator, Task> action)
    {
        return ExecuteScopeAsync(sp => action(sp.GetRequiredService<TWContext>(), sp.GetRequiredService<IMediator>()));
    }

    public Task<T> ExecuteDbContextAsync<T>(Func<TWContext, Task<T>> action)
    {
        return ExecuteScopeAsync(sp => action(sp.GetRequiredService<TWContext>()));
    }

    public Task<T> ExecuteDbContextAsync<T>(Func<TWContext, ValueTask<T>> action)
    {
        return ExecuteScopeAsync(sp => action(sp.GetRequiredService<TWContext>()).AsTask());
    }

    public Task<T> ExecuteDbContextAsync<T>(Func<TWContext, IMediator, Task<T>> action)
    {
        return ExecuteScopeAsync(sp => action(sp.GetRequiredService<TWContext>(), sp.GetRequiredService<IMediator>()));
    }

    public Task InsertAsync<T>(params T[] entities)
        where T : class
    {
        return ExecuteDbContextAsync(db =>
        {
            foreach (var entity in entities)
            {
                db.Set<T>().Add(entity);
            }

            return db.SaveChangesAsync();
        });
    }

    public async Task InsertAsync<TEntity>(TEntity entity)
        where TEntity : class
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

    public Task InsertAsync<TEntity, TEntity2, TEntity3, TEntity4>(
        TEntity entity,
        TEntity2 entity2,
        TEntity3 entity3,
        TEntity4 entity4
    )
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

    public Task<T> FindAsync<T, TKey>(TKey id)
        where T : class, IEntity
    {
        return ExecuteDbContextAsync(db => db.Set<T>().FindAsync(id).AsTask());
    }

    public Task<T> FirstOrDefaultAsync<T>()
        where T : class, IEntity
    {
        return ExecuteDbContextAsync(db => db.Set<T>().FirstOrDefaultAsync());
    }
}

public class TestReadFixture<TEntryPoint, TRContext> : TestFixture<TEntryPoint>
    where TEntryPoint : class
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

    public async Task InsertMongoDbContextAsync<T>(string collectionName, params T[] entities)
        where T : class
    {
        await ExecuteReadContextAsync(async db =>
        {
            await db.GetCollection<T>(collectionName).InsertManyAsync(entities.ToList());
        });
    }
}

public class TestFixture<TEntryPoint, TWContext, TRContext> : TestWriteFixture<TEntryPoint, TWContext>
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

    public async Task InsertMongoDbContextAsync<T>(string collectionName, params T[] entities)
        where T : class
    {
        await ExecuteReadContextAsync(async db =>
        {
            await db.GetCollection<T>(collectionName).InsertManyAsync(entities.ToList());
        });
    }
}

public class TestFixtureCore<TEntryPoint> : IAsyncLifetime
    where TEntryPoint : class
{
    private Respawner _reSpawnerDefaultDb;
    private NpgsqlConnection DefaultDbConnection { get; set; }
    private Type _dbContextType;

    public TestFixtureCore(
        TestFixture<TEntryPoint> integrationTestFixture,
        ITestOutputHelper outputHelper,
        Type dbContextType = null
    )
    {
        Fixture = integrationTestFixture;
        integrationTestFixture.RegisterServices(RegisterTestsServices);
        integrationTestFixture.Logger = integrationTestFixture.CreateLogger(outputHelper);
        _dbContextType = dbContextType;
    }

    public TestFixture<TEntryPoint> Fixture { get; }

    public async Task InitializeAsync()
    {
        await Fixture.EnsureInitializedAsync();
        await InitPostgresAsync();
    }

    public async Task DisposeAsync()
    {
        await ResetPostgresAsync();
        await ResetMongoAsync();
        await ResetRabbitMqAsync();
    }

    private async Task InitPostgresAsync()
    {
        var postgresOptions = Fixture.ServiceProvider.GetService<PostgresOptions>();

        if (!string.IsNullOrEmpty(postgresOptions?.ConnectionString) && _dbContextType != null)
        {
            DefaultDbConnection = new NpgsqlConnection(postgresOptions.ConnectionString);
            await DefaultDbConnection.OpenAsync();

            using var scope = Fixture.ServiceProvider.CreateScope();

            if (scope.ServiceProvider.GetRequiredService(_dbContextType) is DbContext dbContext)
            {
                await dbContext.Database.EnsureCreatedAsync();
            }

            _reSpawnerDefaultDb = await Respawner.CreateAsync(
                DefaultDbConnection,
                new RespawnerOptions { DbAdapter = DbAdapter.Postgres, TablesToIgnore = ["__EFMigrationsHistory"] }
            );

            await SeedDataAsync();
        }
    }

    private async Task ResetPostgresAsync()
    {
        if (DefaultDbConnection is not null)
        {
            await _reSpawnerDefaultDb.ResetAsync(DefaultDbConnection);
        }
    }

    private async Task ResetMongoAsync(CancellationToken cancellationToken = default)
    {
        //https://stackoverflow.com/questions/3366397/delete-everything-in-a-mongodb-database
        var dbClient = new MongoClient(Fixture.MongoDbTestContainer?.GetConnectionString());

        var collections = await dbClient
            .GetDatabase(TestContainers.MongoContainerConfiguration.Name)
            .ListCollectionsAsync(cancellationToken: cancellationToken);

        foreach (var collection in collections.ToList())
        {
            await dbClient
                .GetDatabase(TestContainers.MongoContainerConfiguration.Name)
                .DropCollectionAsync(collection["name"].AsString, cancellationToken);
        }
    }

    private async Task ResetRabbitMqAsync(CancellationToken cancellationToken = default)
    {
        var port =
            Fixture.RabbitMqTestContainer?.GetMappedPublicPort(TestContainers.RabbitMqContainerConfiguration.ApiPort)
            ?? TestContainers.RabbitMqContainerConfiguration.ApiPort;

        var managementClient = new ManagementClient(
            Fixture.RabbitMqTestContainer?.Hostname,
            TestContainers.RabbitMqContainerConfiguration?.UserName,
            TestContainers.RabbitMqContainerConfiguration?.Password,
            port
        );

        var bd = await managementClient.GetBindingsAsync(cancellationToken);

        var bindings = bd.Where(x => !string.IsNullOrEmpty(x.Source) && !string.IsNullOrEmpty(x.Destination));

        foreach (var binding in bindings)
        {
            await managementClient.DeleteBindingAsync(binding, cancellationToken);
        }

        var queues = await managementClient.GetQueuesAsync(cancellationToken: cancellationToken);

        foreach (var queue in queues)
        {
            await managementClient.PurgeAsync(queue, cancellationToken);
        }
    }

    protected virtual void RegisterTestsServices(IServiceCollection services) { }

    private async Task SeedDataAsync()
    {
        using var scope = Fixture.ServiceProvider.CreateScope();

        var seedManager = scope.ServiceProvider.GetService<ISeedManager>();
        await seedManager.ExecuteTestSeedAsync();
    }
}

public abstract class TestReadBase<TEntryPoint, TRContext> : TestFixtureCore<TEntryPoint>
    // ,IClassFixture<IntegrationTestFactory<TEntryPoint, TWContext>>
    where TEntryPoint : class
    where TRContext : MongoDbContext
{
    protected TestReadBase(
        TestReadFixture<TEntryPoint, TRContext> integrationTestFixture,
        ITestOutputHelper outputHelper = null
    )
        : base(integrationTestFixture, outputHelper)
    {
        Fixture = integrationTestFixture;
    }

    public TestReadFixture<TEntryPoint, TRContext> Fixture { get; }
}

public abstract class TestWriteBase<TEntryPoint, TWContext> : TestFixtureCore<TEntryPoint>
    //,IClassFixture<IntegrationTestFactory<TEntryPoint, TWContext>>
    where TEntryPoint : class
    where TWContext : DbContext
{
    protected TestWriteBase(
        TestWriteFixture<TEntryPoint, TWContext> integrationTestFixture,
        ITestOutputHelper outputHelper = null
    )
        : base(integrationTestFixture, outputHelper, typeof(TWContext))
    {
        Fixture = integrationTestFixture;
    }

    public TestWriteFixture<TEntryPoint, TWContext> Fixture { get; }
}

public abstract class TestBase<TEntryPoint, TWContext, TRContext> : TestFixtureCore<TEntryPoint>
    //,IClassFixture<IntegrationTestFactory<TEntryPoint, TWContext, TRContext>>
    where TEntryPoint : class
    where TWContext : DbContext
    where TRContext : MongoDbContext
{
    protected TestBase(
        TestFixture<TEntryPoint, TWContext, TRContext> integrationTestFixture,
        ITestOutputHelper outputHelper = null
    )
        : base(integrationTestFixture, outputHelper, typeof(TWContext))
    {
        Fixture = integrationTestFixture;
    }

    public TestFixture<TEntryPoint, TWContext, TRContext> Fixture { get; }
}
