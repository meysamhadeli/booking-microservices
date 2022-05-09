using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BuildingBlocks.Domain.Model;
using Flight.Data;
using MediatR;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Respawn;
using Xunit;

namespace Integration.Test;

[CollectionDefinition(nameof(TestFixture))]
public class SliceFixtureCollection : ICollectionFixture<TestFixture>
{
}

// ref: https://github.com/jbogard/ContosoUniversityDotNetCore-Pages/blob/master/ContosoUniversity.IntegrationTests/SliceFixture.cs
public class TestFixture : IAsyncLifetime
{
    private readonly Checkpoint _checkpoint;
    private readonly IConfiguration _configuration;
    private readonly WebApplicationFactory<Program> _factory;
    private readonly IServiceScopeFactory _scopeFactory;

    public TestFixture()
    {
        var factory = FlightTestApplicationFactory();

        _configuration = factory.Services.GetRequiredService<IConfiguration>();
        _scopeFactory = factory.Services.GetRequiredService<IServiceScopeFactory>();

        _checkpoint = new Checkpoint();
    }


    public Task InitializeAsync()
    {
        return _checkpoint.Reset(_configuration.GetConnectionString("DefaultConnection"));
    }

    public Task DisposeAsync()
    {
        _factory?.Dispose();
        return Task.CompletedTask;
    }

    public WebApplicationFactory<Program> FlightTestApplicationFactory()
    {
        return new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureAppConfiguration((_, configBuilder) =>
                {
                    configBuilder.AddInMemoryCollection(new Dictionary<string, string>
                    {
                        {
                            "ConnectionStrings:DefaultConnection",
                            "Server=db;Database=FlightDB;User ID=sa;Password=@Aa123456"
                        }
                    });
                });
            });
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
}
