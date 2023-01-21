using System.Reflection;
using BuildingBlocks.EFCore;
using Microsoft.EntityFrameworkCore;

namespace Passenger.Data;

using Microsoft.AspNetCore.Http;

public sealed class PassengerDbContext : AppDbContextBase
{
    public PassengerDbContext(DbContextOptions<PassengerDbContext> options,
        IHttpContextAccessor httpContextAccessor = default) :
        base(options, httpContextAccessor)
    {
    }

    public DbSet<Passengers.Models.Passenger> Passengers => Set<Passengers.Models.Passenger>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(builder);
        builder.ToSnakeCaseTables();
    }
}
