using BuildingBlocks.EFCore;
using BuildingBlocks.PersistMessageProcessor.Data.Configurations;
using Microsoft.EntityFrameworkCore;

namespace BuildingBlocks.PersistMessageProcessor.Data;

public class PersistMessageDbContext : AppDbContextBase, IPersistMessageDbContext
{
    public PersistMessageDbContext(DbContextOptions<PersistMessageDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfiguration(new PersistMessageConfiguration());
        base.OnModelCreating(builder);
    }
}
