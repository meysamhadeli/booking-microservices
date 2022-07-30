using System.Reflection;
using BuildingBlocks.EFCore;
using BuildingBlocks.Utils;
using Flight.Aircrafts.Models;
using Flight.Airports.Models;
using Flight.Seats.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Flight.Data;

public sealed class FlightDbContext : AppDbContextBase
{
    public const string DefaultSchema = "dbo";
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
        builder.FilterSoftDeletedProperties();
        builder.ApplyConfigurationsFromAssembly(typeof(FlightRoot).Assembly);
        base.OnModelCreating(builder);
    }
}
