using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BuildingBlocks.Domain.Model;
using Flight.Data;
using MassTransit;
using MassTransit.Testing;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using Respawn;
using Xunit;
using Xunit.Abstractions;

namespace Integration.Test;

[CollectionDefinition(nameof(TestFixture))]
public class SliceFixtureCollection : ICollectionFixture<TestFixture>
{
}

// ref: https://github.com/jbogard/ContosoUniversityDotNetCore-Pages/blob/master/ContosoUniversity.IntegrationTests/SliceFixture.cs
// ref: https://github.com/MassTransit/MassTransit/blob/00d6992286911a437b63b93c89a56e920b053c11/src/MassTransit.TestFramework/InMemoryTestFixture.cs
// ref: https://wrapt.dev/blog/building-an-event-driven-dotnet-application-integration-testing
public class TestFixture : IAsyncLifetime
{
    private readonly Checkpoint _checkpoint;
    private readonly IConfiguration _configuration;
    private readonly WebApplicationFactory<Program> _factory;
    private readonly IServiceScopeFactory _scopeFactory;
    private static InMemoryTestHarness _harness;
    public ITestOutputHelper Output { get; set; }

    public TestFixture()
    {
        _factory = FlightTestApplicationFactory();

        _configuration = _factory.Services.GetRequiredService<IConfiguration>();
        _scopeFactory = _factory.Services.GetRequiredService<IServiceScopeFactory>();

        _checkpoint = new Checkpoint();
    }

    public WebApplicationFactory<Program> FlightTestApplicationFactory()
    {
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "test");

        return new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices((services) =>
                {
                    services.RemoveAll(typeof(IHostedService));
                });

                builder.ConfigureServices(services =>
                {
                    builder.ConfigureLogging(logging =>
                    {
                        logging.ClearProviders(); // Remove other loggers
                    });

                    var httpContextAccessorService = services.FirstOrDefault(d =>
                        d.ServiceType == typeof(IHttpContextAccessor));

                    services.Remove(httpContextAccessorService);
                    services.AddSingleton(_ => Mock.Of<IHttpContextAccessor>());

                    services.AddScoped<InMemoryTestHarness>();
                    var provider = services.BuildServiceProvider();
                    var serviceScopeFactory = provider.GetService<IServiceScopeFactory>();

                    // MassTransit Start Setup -- Do Not Delete Comment
                    _harness = serviceScopeFactory?.CreateScope().ServiceProvider.GetRequiredService<InMemoryTestHarness>();
                    _harness?.Start().GetAwaiter().GetResult();
                });
            });
    }

    public Task InitializeAsync()
    {
        return _checkpoint.Reset(_configuration.GetConnectionString("DefaultConnection"));
    }

    public async Task DisposeAsync()
    {
        await _harness.Stop();
        await _factory.DisposeAsync();
    }

    public async Task ExecuteScopeAsync(Func<IServiceProvider, Task> action)
    {
        using var scope = _scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<FlightDbContext>();

        try
        {
            await dbContext.BeginTransactionAsync();

            await action(scope.ServiceProvider);

            await dbContext.CommitTransactionAsync();
        }
        catch (Exception)
        {
            await dbContext.RollbackTransactionAsync();
            throw;
        }
    }

    public async Task<T> ExecuteScopeAsync<T>(Func<IServiceProvider, Task<T>> action)
    {
        using var scope = _scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<FlightDbContext>();

        try
        {
            await dbContext.BeginTransactionAsync();

            var result = await action(scope.ServiceProvider);

            await dbContext.CommitTransactionAsync();

            return result;
        }
        catch (Exception)
        {
            await dbContext.RollbackTransactionAsync();
            throw;
        }
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

    public Task<T> FindAsync<T>(int id)
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


        // MassTransit Methods -- Do Not Delete Comment
    /// <summary>
    ///     Publishes a message to the bus, and waits for the specified response.
    /// </summary>
    /// <param name="message">The message that should be published.</param>
    /// <typeparam name="TMessage">The message that should be published.</typeparam>
    public static async Task PublishMessage<TMessage>(object message)
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
}
