using System.Reflection;
using BookingMonolith.Flight.Aircrafts.Models;
using BookingMonolith.Flight.Airports.Models;
using BookingMonolith.Flight.Seats.Models;
using BuildingBlocks.EFCore;
using BuildingBlocks.Web;
using Humanizer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BookingMonolith.Flight.Data;

public sealed class FlightDbContext : AppDbContextBase
{
    public FlightDbContext(DbContextOptions<FlightDbContext> options, ICurrentUserProvider? currentUserProvider = null,
        ILogger<FlightDbContext>? logger = null) : base(
        options, currentUserProvider, logger)
    {
    }

    public DbSet<Flights.Models.Flight> Flights => Set<Flights.Models.Flight>();
    public DbSet<Airport> Airports => Set<Airport>();
    public DbSet<Aircraft> Aircraft => Set<Aircraft>();
    public DbSet<Seat> Seats => Set<Seat>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        var types = typeof(FlightRoot).Assembly.GetTypes()
            .Where(t => t.GetCustomAttribute<RegisterFlightConfigurationAttribute>() != null)
            .ToList();

        foreach (var type in types)
        {
            dynamic configuration = Activator.CreateInstance(type)!;
            builder.ApplyConfiguration(configuration);
        }

        builder.HasDefaultSchema(nameof(Flight).Underscore());
        builder.FilterSoftDeletedProperties();
        builder.ToSnakeCaseTables();
    }
}
