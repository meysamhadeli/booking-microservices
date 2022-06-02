using System;
using System.Net.Http;
using System.Threading.Tasks;
using BuildingBlocks.Domain.Model;
using BuildingBlocks.EFCore;
using BuildingBlocks.MassTransit;
using BuildingBlocks.Web;
using Grpc.Net.Client;
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
using NSubstitute;
using Passenger.Data;
using Respawn;
using Serilog;
using Xunit;
using Xunit.Abstractions;

namespace Integration.Test;

[CollectionDefinition(nameof(TestFixture))]
public class TestFixtureCollection : ICollectionFixture<TestFixture>
{
}

// ref: https://docs.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-6.0
// ref: https://github.com/jbogard/ContosoUniversityDotNetCore-Pages/blob/master/ContosoUniversity.IntegrationTests/SliceFixture.cs
// ref: https://github.com/jasontaylordev/CleanArchitecture/blob/main/tests/Application.IntegrationTests/Testing.cs
// ref: https://github.com/MassTransit/MassTransit/blob/00d6992286911a437b63b93c89a56e920b053c11/src/MassTransit.TestFramework/InMemoryTestFixture.cs
public class TestFixture : IAsyncLifetime
{
    private Checkpoint _checkpoint;
    private IConfiguration _configuration;
    private WebApplicationFactory<Program> _factory;
    private HttpClient _httpClient;
    private IServiceScopeFactory _scopeFactory;
    private GrpcChannel _channel;
    public ITestHarness TestHarness { get; private set; }
    public GrpcChannel Channel => _channel;

    public async Task InitializeAsync()
    {
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "test");

        _factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder => builder.ConfigureServices(services =>
            {
                services.RemoveAll(typeof(IHostedService));
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
            }));

        TestHarness = _factory.Services.GetTestHarness();

        await TestHarness.Start();

        _configuration = _factory.Services.GetRequiredService<IConfiguration>();
        _scopeFactory = _factory.Services.GetRequiredService<IServiceScopeFactory>();

        _httpClient = _factory.CreateClient(new WebApplicationFactoryClientOptions {AllowAutoRedirect = false});

        _channel = GrpcChannel.ForAddress(_httpClient.BaseAddress!, new GrpcChannelOptions {HttpClient = _httpClient});

        _checkpoint = new Checkpoint {TablesToIgnore = new[] {"__EFMigrationsHistory"}};

        await EnsureDatabaseAsync();
    }

    public async Task DisposeAsync()
    {
        TestHarness.Cancel();
        await _factory.DisposeAsync();
        await _checkpoint.Reset(_configuration.GetConnectionString("DefaultConnection"));
    }

    // ref: https://github.com/trbenning/serilog-sinks-xunit
    public ILogger CreateLogger(ITestOutputHelper output)
    {
        if (output != null)
            return new LoggerConfiguration()
                .WriteTo.TestOutput(output)
                .CreateLogger();

        return null;
    }

    public HttpClient CreateClient()
    {
        return _httpClient;
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

    public Task ExecuteDbContextAsync(Func<PassengerDbContext, Task> action)
    {
        return ExecuteScopeAsync(sp => action(sp.GetService<PassengerDbContext>()));
    }

    public Task ExecuteDbContextAsync(Func<PassengerDbContext, ValueTask> action)
    {
        return ExecuteScopeAsync(sp => action(sp.GetService<PassengerDbContext>()).AsTask());
    }

    public Task ExecuteDbContextAsync(Func<PassengerDbContext, IMediator, Task> action)
    {
        return ExecuteScopeAsync(sp => action(sp.GetService<PassengerDbContext>(), sp.GetService<IMediator>()));
    }

    public Task<T> ExecuteDbContextAsync<T>(Func<PassengerDbContext, Task<T>> action)
    {
        return ExecuteScopeAsync(sp => action(sp.GetService<PassengerDbContext>()));
    }

    public Task<T> ExecuteDbContextAsync<T>(Func<PassengerDbContext, ValueTask<T>> action)
    {
        return ExecuteScopeAsync(sp => action(sp.GetService<PassengerDbContext>()).AsTask());
    }

    public Task<T> ExecuteDbContextAsync<T>(Func<PassengerDbContext, IMediator, Task<T>> action)
    {
        return ExecuteScopeAsync(sp => action(sp.GetService<PassengerDbContext>(), sp.GetService<IMediator>()));
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

    private async Task EnsureDatabaseAsync()
    {
        using var scope = _scopeFactory.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<PassengerDbContext>();
        var seeders = scope.ServiceProvider.GetServices<IDataSeeder>();

        await context.Database.MigrateAsync();

        foreach (var seeder in seeders) await seeder.SeedAllAsync();
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
