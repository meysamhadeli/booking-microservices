using System.Reflection;
using BuildingBlocks.EFCore;
using BuildingBlocks.Web;
using Humanizer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BookingMonolith.Passenger.Data;

public sealed class PassengerDbContext : AppDbContextBase
{
    public PassengerDbContext(DbContextOptions<PassengerDbContext> options,
        ICurrentUserProvider? currentUserProvider = null, ILogger<PassengerDbContext>? logger = null) :
        base(options, currentUserProvider, logger)
    {
    }

    public DbSet<Passengers.Models.Passenger> Passengers => Set<Passengers.Models.Passenger>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        var types = typeof(PassengerRoot).Assembly.GetTypes()
            .Where(t => t.GetCustomAttribute<RegisterPassengerConfigurationAttribute>() != null)
            .ToList();

        foreach (var type in types)
        {
            dynamic configuration = Activator.CreateInstance(type)!;
            builder.ApplyConfiguration(configuration);
        }

        builder.HasDefaultSchema(nameof(Passenger).Underscore());
        base.OnModelCreating(builder);
        builder.FilterSoftDeletedProperties();
        builder.ToSnakeCaseTables();
    }
}
