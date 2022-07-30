using BuildingBlocks.EFCore;
using Microsoft.EntityFrameworkCore;

namespace BuildingBlocks.PersistMessageProcessor;

public interface IPersistMessageDbContext : IDbContext
{
    DbSet<PersistMessage> PersistMessages => Set<PersistMessage>();
}
