using BuildingBlocks.EFCore;
using Microsoft.EntityFrameworkCore;

namespace BuildingBlocks.PersistMessageProcessor.Data;

using System.Net;
using Configurations;
using Core.Model;
using global::Polly;
using Microsoft.Extensions.Logging;
using Exception = System.Exception;

public class PersistMessageDbContext : DbContext, IPersistMessageDbContext
{
    private readonly Lazy<ILogger<PersistMessageDbContext>> _logger;

    public PersistMessageDbContext(DbContextOptions<PersistMessageDbContext> options)
        : base(options)
    {
        _logger = new Lazy<ILogger<PersistMessageDbContext>>(() =>
        {
            var factory = LoggerFactory.Create(b => b.AddConsole());
            var logger = factory.CreateLogger<PersistMessageDbContext>();
            return logger;
        });
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

        var policy = Policy.Handle<DbUpdateConcurrencyException>()
            .WaitAndRetryAsync(retryCount: 3,
                sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(1),
                onRetry: (exception, timeSpan, retryCount, context) =>
                {
                    if (exception != null)
                    {
                        _logger.Value.LogError(exception,
                            "Request failed with {StatusCode}. Waiting {TimeSpan} before next retry. Retry attempt {RetryCount}.",
                            HttpStatusCode.Conflict,
                            timeSpan,
                            retryCount);
                    }
                });
        try
        {
            return await policy.ExecuteAsync(async () => await base.SaveChangesAsync(cancellationToken));
        }
        catch (DbUpdateConcurrencyException ex)
        {
            foreach (var entry in ex.Entries)
            {
                var currentValue = entry.Entity; // we can use it for specific merging
                var databaseValue = await entry.GetDatabaseValuesAsync(cancellationToken);

                _logger.Value.LogInformation(
                    "Entry to entity with type: {Type}, database-value: {DatabaseValue} and current-value: {CurrentValue}",
                    entry.GetType().Name,
                    databaseValue,
                    currentValue);

                if (databaseValue != null)
                {
                    entry.OriginalValues.SetValues(databaseValue);
                }
                else
                {
                    entry.OriginalValues.SetValues(currentValue);
                }
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
