using System.Collections.Immutable;
using BuildingBlocks.Core.Event;
using BuildingBlocks.Core.Model;
using BuildingBlocks.Web;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace BuildingBlocks.EFCore;

public abstract class AppDbContextBase : DbContext, IDbContext
{
    private readonly ICurrentUserProvider _currentUserProvider;

    private IDbContextTransaction _currentTransaction;

    protected AppDbContextBase(DbContextOptions options, ICurrentUserProvider currentUserProvider = null) : base(options)
    {
        _currentUserProvider = currentUserProvider;
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

        _currentTransaction ??= await Database.BeginTransactionAsync(cancellationToken);
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

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        OnBeforeSaving();
        return base.SaveChangesAsync(cancellationToken);
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
        foreach (var entry in ChangeTracker.Entries<IAggregate>())
        {
            var isAuditable = entry.Entity.GetType().IsAssignableTo(typeof(IAggregate));
            var userId = _currentUserProvider?.GetCurrentUserId();

            if (isAuditable)
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedBy = userId;
                        entry.Entity.CreatedAt = DateTime.Now;
                        entry.Entity.Version++;
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
}
