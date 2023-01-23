namespace BuildingBlocks.EFCore;

using System.Collections.Immutable;
using BuildingBlocks.Core.Event;
using BuildingBlocks.Core.Model;
using Microsoft.EntityFrameworkCore;
using System.Data;
using Web;
using Exception = System.Exception;

public abstract class AppDbContextBase : DbContext, IDbContext
{
    private readonly ICurrentUserProvider _currentUserProvider;

    protected AppDbContextBase(DbContextOptions options, ICurrentUserProvider currentUserProvider) :
        base(options)
    {
        _currentUserProvider = currentUserProvider;
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
    }

    //ref: https://learn.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency#execution-strategies-and-transactions
    public Task ExecuteTransactionalAsync(CancellationToken cancellationToken = default)
    {
        var strategy = Database.CreateExecutionStrategy();
        return strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await Database.BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken);
            try
            {
                await SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        });
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        OnBeforeSaving();
        return await base.SaveChangesAsync(cancellationToken);
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
                var userId = _currentUserProvider?.GetCurrentUserId() ?? 0;

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
}
