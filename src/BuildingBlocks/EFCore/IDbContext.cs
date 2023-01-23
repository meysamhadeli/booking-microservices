using BuildingBlocks.Core.Event;
using Microsoft.EntityFrameworkCore;

namespace BuildingBlocks.EFCore;

public interface IDbContext
{
    DbSet<TEntity> Set<TEntity>() where TEntity : class;
    IReadOnlyList<IDomainEvent> GetDomainEvents();
    Task ExecuteTransactionalAsync(CancellationToken cancellationToken = default);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
