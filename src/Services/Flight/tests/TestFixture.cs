using System;
using System.Net.Http;
using System.Threading.Tasks;
using BuildingBlocks.Domain.Model;
using BuildingBlocks.EFCore;
using BuildingBlocks.MassTransit;
using BuildingBlocks.Web;
using Flight.Data;
using Flight.Data.Seed;
using MassTransit;
using MassTransit.Testing;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Respawn;
using Xunit;

namespace Integration.Test;

[CollectionDefinition(nameof(TestFixture))]
public class TestFixtureCollection : ICollectionFixture<TestFixture>
{
}

// ref: https://docs.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-6.0
// ref: https://github.com/jbogard/ContosoUniversityDotNetCore-Pages/blob/master/ContosoUniversity.IntegrationTests/SliceFixture.cs
// ref: https://github.com/jasontaylordev/CleanArchitecture/blob/main/tests/Application.IntegrationTests/Testing.cs
public class TestFixture : IAsyncLifetime
{
    private Checkpoint _checkpoint;
    private HttpClient _client;
    private IConfiguration _configuration;
    private WebApplicationFactory<Program> _factory;
    private ITestHarness _harness;
    private IServiceScopeFactory _scopeFactory;

    public ILogger<TestFixture> Logger =>
        _scopeFactory.CreateScope().ServiceProvider.GetRequiredService<ILogger<TestFixture>>();


    public async Task InitializeAsync()
    {
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "test");

        _factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder => builder.ConfigureServices(services =>
            {
                services.RemoveAll(typeof(IHostedService));
                services.ReplaceSingleton(AddHttpContextAccessorMock);
                services.ReplaceScoped<IDataSeeder, FlightDataSeeder>();
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
            }));

        _harness = _factory.Services.GetTestHarness();

        await _harness.Start();

        _configuration = _factory.Services.GetRequiredService<IConfiguration>();
        _scopeFactory = _factory.Services.GetRequiredService<IServiceScopeFactory>();

        _client = _factory.CreateClient(new WebApplicationFactoryClientOptions {AllowAutoRedirect = false});

        _checkpoint = new Checkpoint {TablesToIgnore = new[] {"__EFMigrationsHistory"}};

        await EnsureDatabaseAsync();
    }

    public async Task DisposeAsync()
    {
        _harness.Cancel();
        await _factory.DisposeAsync();
        await _checkpoint.Reset(_configuration.GetConnectionString("DefaultConnection"));
    }


    public async Task ExecuteScopeAsync(Func<IServiceProvider, Task> action)
    {
        using var scope = _scopeFactory.CreateScope();
        await action(scope.ServiceProvider);
    }

    public async Task<T> ExecuteScopeAsync<T>(Func<IServiceProvider, Task<T>> action)
    {
        using var scope = _scopeFactory.CreateScope();

        var result = await action(scope.ServiceProvider);

        return result;
    }

    public Task ExecuteDbContextAsync(Func<FlightDbContext, Task> action)
    {
        return ExecuteScopeAsync(sp => action(sp.GetService<FlightDbContext>()));
    }

    public Task ExecuteDbContextAsync(Func<FlightDbContext, ValueTask> action)
    {
        return ExecuteScopeAsync(sp => action(sp.GetService<FlightDbContext>()).AsTask());
    }

    public Task ExecuteDbContextAsync(Func<FlightDbContext, IMediator, Task> action)
    {
        return ExecuteScopeAsync(sp => action(sp.GetService<FlightDbContext>(), sp.GetService<IMediator>()));
    }

    public Task<T> ExecuteDbContextAsync<T>(Func<FlightDbContext, Task<T>> action)
    {
        return ExecuteScopeAsync(sp => action(sp.GetService<FlightDbContext>()));
    }

    public Task<T> ExecuteDbContextAsync<T>(Func<FlightDbContext, ValueTask<T>> action)
    {
        return ExecuteScopeAsync(sp => action(sp.GetService<FlightDbContext>()).AsTask());
    }

    public Task<T> ExecuteDbContextAsync<T>(Func<FlightDbContext, IMediator, Task<T>> action)
    {
        return ExecuteScopeAsync(sp => action(sp.GetService<FlightDbContext>(), sp.GetService<IMediator>()));
    }

    public Task InsertAsync<T>(params T[] entities) where T : class
    {
        return ExecuteDbContextAsync(db =>
        {
            foreach (var entity in entities) db.Set<T>().Add(entity);

            return db.SaveChangesAsync();
        });
    }

    public Task InsertAsync<TEntity>(TEntity entity) where TEntity : class
    {
        return ExecuteDbContextAsync(db =>
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
        where T : class, IEntity
    {
        return ExecuteDbContextAsync(db => db.Set<T>().FindAsync(id).AsTask());
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


    // ref: https://github.com/MassTransit/MassTransit/blob/00d6992286911a437b63b93c89a56e920b053c11/src/MassTransit.TestFramework/InMemoryTestFixture.cs
    // ref: https://wrapt.dev/blog/building-an-event-driven-dotnet-application-integration-testing

    /// <summary>
    ///     Publishes a message to the bus, and waits for the specified response.
    /// </summary>
    /// <param name="message">The message that should be published.</param>
    /// <typeparam name="TMessage">The message that should be published.</typeparam>
    public async Task PublishMessage<TMessage>(object message)
        where TMessage : class
    {
        await _harness.Bus.Publish<TMessage>(message);
    }

    /// <summary>
    ///     Confirm if there was a fault when publishing for this harness.
    /// </summary>
    /// <typeparam name="TMessage">The message that should be published.</typeparam>
    /// <returns>A boolean of true if there was a fault for a message of the given type when published.</returns>
    public Task<bool> IsFaultyPublished<TMessage>()
        where TMessage : class
    {
        return _harness.Published.Any<Fault<TMessage>>();
    }

    /// <summary>
    ///     Confirm that a message has been published for this harness.
    /// </summary>
    /// <typeparam name="TMessage">The message that should be published.</typeparam>
    /// <returns>A boolean of true if a message of the given type has been published.</returns>
    public Task<bool> IsPublished<TMessage>()
        where TMessage : class
    {
        return _harness.Published.Any<TMessage>();
    }

    /// <summary>
    ///     Confirm that a message has been consumed for this harness.
    /// </summary>
    /// <typeparam name="TMessage">The message that should be consumed.</typeparam>
    /// <returns>A boolean of true if a message of the given type has been consumed.</returns>
    public Task<bool> IsConsumed<TMessage>()
        where TMessage : class
    {
        return _harness.Consumed.Any<TMessage>();
    }

    /// <summary>
    ///     The desired consumer consumed the message.
    /// </summary>
    /// <typeparam name="TMessage">The message that should be consumed.</typeparam>
    /// <typeparam name="TConsumedBy">The consumer of the message.</typeparam>
    /// <returns>A boolean of true if a message of the given type has been consumed by the given consumer.</returns>
    public Task<bool> IsConsumed<TMessage, TConsumedBy>()
        where TMessage : class
        where TConsumedBy : class, IConsumer
    {
        using var scope = _scopeFactory.CreateScope();
        var consumerHarness = scope.ServiceProvider.GetRequiredService<IConsumerTestHarness<TConsumedBy>>();
        return consumerHarness.Consumed.Any<TMessage>();
    }

    private async Task EnsureDatabaseAsync()
    {
        using var scope = _scopeFactory.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<FlightDbContext>();
        var seeders = scope.ServiceProvider.GetServices<IDataSeeder>();

        await context.Database.MigrateAsync();

        foreach (var seeder in seeders) await seeder.SeedAllAsync();
    }

    // private async Task AddInMemoryHarnessAsync()
    // {
    //     _harness = _factory.Services.GetRequiredService<InMemoryTestHarness>();
    //     await _harness.Start();
    // }

    private IHttpContextAccessor AddHttpContextAccessorMock(IServiceProvider serviceProvider)
    {
        var httpContextAccessorMock = Substitute.For<IHttpContextAccessor>();
        using var scope = serviceProvider.CreateScope();
        httpContextAccessorMock.HttpContext = new DefaultHttpContext {RequestServices = scope.ServiceProvider};

        httpContextAccessorMock.HttpContext.Request.Host = new HostString("localhost", 5000);
        httpContextAccessorMock.HttpContext.Request.Scheme = "http";

        return httpContextAccessorMock;
    }
}
