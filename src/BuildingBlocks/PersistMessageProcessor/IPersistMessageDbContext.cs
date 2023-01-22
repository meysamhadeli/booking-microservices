using Microsoft.EntityFrameworkCore;

namespace BuildingBlocks.PersistMessageProcessor;

using EFCore;

public interface IPersistMessageDbContext
{
    DbSet<PersistMessage> PersistMessages { get; set; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
