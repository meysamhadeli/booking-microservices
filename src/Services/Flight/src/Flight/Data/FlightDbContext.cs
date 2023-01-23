using BuildingBlocks.EFCore;
using Flight.Aircrafts.Models;
using Flight.Airports.Models;
using Flight.Seats.Models;
using Microsoft.EntityFrameworkCore;

namespace Flight.Data;

using BuildingBlocks.Web;

public sealed class FlightDbContext : AppDbContextBase
{
    public FlightDbContext(DbContextOptions<FlightDbContext> options, ICurrentUserProvider currentUserProvider) : base(
        options, currentUserProvider)
    {
    }

    public DbSet<Flights.Models.Flight> Flights => Set<Flights.Models.Flight>();
    public DbSet<Airport> Airports => Set<Airport>();
    public DbSet<Aircraft> Aircraft => Set<Aircraft>();
    public DbSet<Seat> Seats => Set<Seat>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(FlightRoot).Assembly);
        builder.FilterSoftDeletedProperties();
        builder.ToSnakeCaseTables();
    }
}
