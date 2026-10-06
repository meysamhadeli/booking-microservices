using System.Reflection;
using Griffin.EFCore;
using Griffin.Web;
using Microsoft.EntityFrameworkCore;

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