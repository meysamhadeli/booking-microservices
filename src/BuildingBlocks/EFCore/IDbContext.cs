using System.Data;
using BuildingBlocks.Core.Event;
using BuildingBlocks.MessageProcessor;
using Microsoft.EntityFrameworkCore;

namespace BuildingBlocks.EFCore;

public interface IDbContext
{
    DbSet<TEntity> Set<TEntity>()
        where TEntity : class;
    DbSet<PersistMessage> PersistMessages => Set<PersistMessage>();
    IReadOnlyList<IDomainEvent> GetDomainEvents();
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
