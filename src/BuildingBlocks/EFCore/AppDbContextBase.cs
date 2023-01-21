using System.Collections.Immutable;
using BuildingBlocks.Core.Event;
using BuildingBlocks.Core.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace BuildingBlocks.EFCore;

using System.Data;
using System.Net;
using System.Security.Claims;
using global::Polly;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Exception = System.Exception;

public abstract class AppDbContextBase : DbContext, IDbContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private IDbContextTransaction _currentTransaction;

    protected AppDbContextBase(DbContextOptions options, IHttpContextAccessor httpContextAccessor = default) :
        base(options)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        // ref: https://github.com/pdevito3/MessageBusTestingInMemHarness/blob/main/RecipeManagement/src/RecipeManagement/Databases/RecipesDbContext.cs
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_currentTransaction != null)
        {
            return;
        }

        _currentTransaction ??= await Database.BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await SaveChangesAsync(cancellationToken);
            await _currentTransaction?.CommitAsync(cancellationToken)!;
        }
        catch
        {
            await RollbackTransactionAsync(cancellationToken);
            throw;
        }
        finally
        {
            _currentTransaction?.Dispose();
            _currentTransaction = null;
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await _currentTransaction?.RollbackAsync(cancellationToken)!;
        }
        finally
        {
            _currentTransaction?.Dispose();
            _currentTransaction = null;
        }
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        OnBeforeSaving();
        try
        {
            return await base.SaveChangesAsync(cancellationToken);
        }
        //ref: https://learn.microsoft.com/en-us/ef/core/saving/concurrency?tabs=fluent-api#resolving-concurrency-conflicts
        catch (DbUpdateConcurrencyException ex)
        {
            throw new DbUpdateConcurrencyException("try for get unhandled exception with DbUpdateConcurrencyException", ex);
            var logger = _httpContextAccessor?.HttpContext?.RequestServices
                .GetRequiredService<ILogger<AppDbContextBase>>();

            var entry = ex.Entries.SingleOrDefault();

            if (entry == null)
            {
                return 0;
            }

            var currentValue = entry.CurrentValues;
            var databaseValue = await entry.GetDatabaseValuesAsync(cancellationToken);

            logger?.LogInformation("The entity being updated is already use by another Thread!" +
                                   " database value is: {DatabaseValue} and current value is: {CurrentValue}",
                databaseValue, currentValue);

            var policy = Policy.Handle<DbUpdateConcurrencyException>()
                .WaitAndRetryAsync(retryCount: 3,
                    sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(1),
                    onRetry: (exception, timeSpan, retryCount, context) =>
                    {
                        if (exception != null)
                        {
                            logger?.LogError(exception,
                                "Request failed with {StatusCode}. Waiting {TimeSpan} before next retry. Retry attempt {RetryCount}.",
                                HttpStatusCode.Conflict,
                                timeSpan,
                                retryCount);
                        }
                    });

            return await policy.ExecuteAsync(async () => await base.SaveChangesAsync(cancellationToken));
        }
        catch (Exception ex)
        {
            throw new Exception("try for get unhandled exception bt default", ex);
        }
    }

    public IReadOnlyList<IDomainEvent> GetDomainEvents()
    {
        var domainEntities = ChangeTracker
            .Entries<IAggregate>()
            .Where(x => x.Entity.DomainEvents.Any())
            .Select(x => x.Entity)
            .ToList();

        var domainEvents = domainEntities
            .SelectMany(x => x.DomainEvents)
            .ToImmutableList();

        domainEntities.ForEach(entity => entity.ClearDomainEvents());

        return domainEvents.ToImmutableList();
    }

    // ref: https://www.meziantou.net/entity-framework-core-generate-tracking-columns.htm
    // ref: https://www.meziantou.net/entity-framework-core-soft-delete-using-query-filters.htm
    private void OnBeforeSaving()
    {
        try
        {
            foreach (var entry in ChangeTracker.Entries<IAggregate>())
            {
                var isAuditable = entry.Entity.GetType().IsAssignableTo(typeof(IAggregate));
                var userId = GetCurrentUserId();

                if (isAuditable)
                {
                    switch (entry.State)
                    {
                        case EntityState.Added:
                            entry.Entity.CreatedBy = userId;
                            entry.Entity.CreatedAt = DateTime.Now;
                            break;

                        case EntityState.Modified:
                            entry.Entity.LastModifiedBy = userId;
                            entry.Entity.LastModified = DateTime.Now;
                            entry.Entity.Version++;
                            break;

                        case EntityState.Deleted:
                            entry.State = EntityState.Modified;
                            entry.Entity.LastModifiedBy = userId;
                            entry.Entity.LastModified = DateTime.Now;
                            entry.Entity.IsDeleted = true;
                            entry.Entity.Version++;
                            break;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            throw new Exception("try for find IAggregate", ex);
        }
    }

    private long? GetCurrentUserId()
    {
        var nameIdentifier = _httpContextAccessor?.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);

        long.TryParse(nameIdentifier, out var userId);

        return userId;
    }
}
