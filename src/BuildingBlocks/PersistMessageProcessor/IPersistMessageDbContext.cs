using Microsoft.EntityFrameworkCore;

namespace BuildingBlocks.PersistMessageProcessor;

public interface IPersistMessageDbContext
{
    DbSet<PersistMessage> PersistMessages { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
