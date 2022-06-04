using System;
using System.Net.Http;
using System.Threading.Tasks;
using BuildingBlocks.Domain.Model;
using BuildingBlocks.EFCore;
using Grpc.Net.Client;
using MassTransit.Testing;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Passenger.Data;
using Serilog;
using Xunit;
using Xunit.Abstractions;

namespace Integration.Test;

[CollectionDefinition(nameof(IntegrationTestFixture))]
public class SliceFixtureCollection : ICollectionFixture<IntegrationTestFixture> { }

public class IntegrationTestFixture : IAsyncLifetime
{
    private readonly CustomWebApplicationFactory _factory;

    public IntegrationTestFixture()
    {
        // Ref: https://docs.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-6.0#basic-tests-with-the-default-webapplicationfactory
        _factory = new CustomWebApplicationFactory();
    }

    public IServiceProvider ServiceProvider => _factory.Services;
    public IConfiguration Configuration => _factory.Configuration;
    public HttpClient HttpClient => _factory.CreateClient();
    public ITestHarness TestHarness => CreateHarness();
    public GrpcChannel Channel => CreateChannel();

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

    public void RegisterTestServices(Action<IServiceCollection> services) => _factory.TestRegistrationServices = services;

    public virtual Task InitializeAsync()
    {
        return Task.CompletedTask;
    }

    public virtual async Task DisposeAsync()
    {
        if (!string.IsNullOrEmpty(Configuration?.GetConnectionString("DefaultConnection")))
            await _factory.Checkpoint.Reset(Configuration?.GetConnectionString("DefaultConnection"));

        await _factory.DisposeAsync();
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

    private IHttpContextAccessor AddHttpContextAccessorMock(IServiceProvider serviceProvider)
    {
        var httpContextAccessorMock = Substitute.For<IHttpContextAccessor>();
        using var scope = serviceProvider.CreateScope();
        httpContextAccessorMock.HttpContext = new DefaultHttpContext {RequestServices = scope.ServiceProvider};

        httpContextAccessorMock.HttpContext.Request.Host = new HostString("localhost", 5000);
        httpContextAccessorMock.HttpContext.Request.Scheme = "http";

        return httpContextAccessorMock;
    }

    private ITestHarness CreateHarness()
    {
        var harness = ServiceProvider.GetTestHarness();
        harness.Start().GetAwaiter().GetResult();
        return harness;
    }

    private GrpcChannel CreateChannel()
    {
        return GrpcChannel.ForAddress(HttpClient.BaseAddress!, new GrpcChannelOptions {HttpClient = HttpClient});
    }

    private async Task EnsureDatabaseAsync()
    {
        using var scope = ServiceProvider.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<PassengerDbContext>();
        var seeders = scope.ServiceProvider.GetServices<IDataSeeder>();

        await context.Database.MigrateAsync();

        foreach (var seeder in seeders) await seeder.SeedAllAsync();
    }
}
