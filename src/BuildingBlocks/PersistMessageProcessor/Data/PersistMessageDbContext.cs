using BuildingBlocks.EFCore;
using Microsoft.EntityFrameworkCore;

namespace BuildingBlocks.PersistMessageProcessor.Data;

using Configurations;
using Core.Model;
using Microsoft.Extensions.Logging;
using Exception = System.Exception;

public class PersistMessageDbContext : DbContext, IPersistMessageDbContext
{
    private readonly ILogger<PersistMessageDbContext>? _logger;

    public PersistMessageDbContext(DbContextOptions<PersistMessageDbContext> options,
        ILogger<PersistMessageDbContext>? logger = null)
        : base(options)
    {
        _logger = logger;
    }

    public DbSet<PersistMessage> PersistMessages => Set<PersistMessage>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfiguration(new PersistMessageConfiguration());
        base.OnModelCreating(builder);
        builder.ToSnakeCaseTables();
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        OnBeforeSaving();

        try
        {
            return await base.SaveChangesAsync(cancellationToken);
        }
        //ref: https://learn.microsoft.com/en-us/ef/core/saving/concurrency?tabs=data-annotations#resolving-concurrency-conflicts
        catch (DbUpdateConcurrencyException ex)
        {
            foreach (var entry in ex.Entries)
            {
                var databaseValues = await entry.GetDatabaseValuesAsync(cancellationToken);

                if (databaseValues == null)
                {
                    _logger.LogError("The record no longer exists in the database, The record has been deleted by another user.");
                    throw;
                }

                // Refresh the original values to bypass next concurrency check
                entry.OriginalValues.SetValues(databaseValues);
            }

            return await base.SaveChangesAsync(cancellationToken);
        }
    }

    private void OnBeforeSaving()
    {
        try
        {
            foreach (var entry in ChangeTracker.Entries<IVersion>())
            {
                switch (entry.State)
                {
                    case EntityState.Modified:
                        entry.Entity.Version++;
                        break;

                    case EntityState.Deleted:
                        entry.Entity.Version++;
                        break;
                }
            }
        }
        catch (Exception ex)
        {
            throw new Exception("try for find IVersion", ex);
        }
    }
}
