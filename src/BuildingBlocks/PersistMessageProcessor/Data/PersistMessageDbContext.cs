using BuildingBlocks.EFCore;
using BuildingBlocks.PersistMessageProcessor.Data.Configurations;
using Microsoft.EntityFrameworkCore;

namespace BuildingBlocks.PersistMessageProcessor.Data;

using Microsoft.AspNetCore.Http;

public class PersistMessageDbContext : AppDbContextBase, IPersistMessageDbContext
{
    public PersistMessageDbContext(DbContextOptions<PersistMessageDbContext> options, IHttpContextAccessor httpContextAccessor = default)
        : base(options, httpContextAccessor)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfiguration(new PersistMessageConfiguration());
        base.OnModelCreating(builder);
        builder.ToSnakeCaseTables();
    }
}
