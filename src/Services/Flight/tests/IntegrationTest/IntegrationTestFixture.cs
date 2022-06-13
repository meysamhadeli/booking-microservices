using System;
using System.Net.Http;
using System.Threading.Tasks;
using BuildingBlocks.Domain.Model;
using BuildingBlocks.EFCore;
using BuildingBlocks.MassTransit;
using BuildingBlocks.Web;
using Flight.Data;
using FluentAssertions.Common;
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
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using NSubstitute;
using Respawn;
using Serilog;
using Xunit;
using Xunit.Abstractions;

namespace Integration.Test;

[CollectionDefinition(nameof(IntegrationTestFixture))]
public class FixtureCollection : ICollectionFixture<IntegrationTestFixture>
{
}

public class IntegrationTestFixture : IAsyncLifetime
{
    private WebApplicationFactory<Program> _factory;
    public Checkpoint Checkpoint { get; set; }
    public Action<IServiceCollection>? TestRegistrationServices { get; set; }
    public IServiceProvider ServiceProvider => _factory.Services;
    public IConfiguration Configuration => _factory.Services.GetRequiredService<IConfiguration>();
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

    public void RegisterTestServices(Action<IServiceCollection> services) => TestRegistrationServices = services;

    public virtual Task InitializeAsync()
    {
        _factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("test");
                builder.ConfigureServices(services =>
                {
                    services.RemoveAll(typeof(IHostedService));
                    services.ReplaceSingleton(AddHttpContextAccessorMock);
                    TestRegistrationServices?.Invoke(services);
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

                    Checkpoint = new Checkpoint {TablesToIgnore = new[] {"__EFMigrationsHistory"}};

                    TestRegistrationServices?.Invoke(services);
                });
            });

        return Task.CompletedTask;
    }

    public virtual async Task DisposeAsync()
    {
        if (!string.IsNullOrEmpty(Configuration?.GetConnectionString("DefaultConnection")))
            await Checkpoint.Reset(Configuration?.GetConnectionString("DefaultConnection"));

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
