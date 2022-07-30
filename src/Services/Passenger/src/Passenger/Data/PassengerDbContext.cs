using System.Reflection;
using BuildingBlocks.EFCore;
using BuildingBlocks.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Passenger.Data;

public sealed class PassengerDbContext : AppDbContextBase
{
    public const string DefaultSchema = "dbo";

    public PassengerDbContext(DbContextOptions<PassengerDbContext> options, ICurrentUserProvider currentUserProvider) : base(options, currentUserProvider)
    {
    }

    public DbSet<Passengers.Models.Passenger> Passengers => Set<Passengers.Models.Passenger>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(builder);
    }
}
