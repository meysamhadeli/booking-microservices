using System.Reflection;
using BuildingBlocks.EFCore;
using Microsoft.EntityFrameworkCore;
using BuildingBlocks.Web;

namespace Passenger.Data;

using Microsoft.Extensions.Logging;

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
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(builder);
        builder.FilterSoftDeletedProperties();
        builder.ToSnakeCaseTables();
    }
}
