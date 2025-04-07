using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace BookingMonolith.Flight.Data
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<FlightDbContext>
    {
        public FlightDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<FlightDbContext>();

            builder.UseNpgsql("Server=localhost;Port=5432;Database=booking_monolith;User Id=postgres;Password=postgres;Include Error Detail=true")
                .UseSnakeCaseNamingConvention();
            return new FlightDbContext(builder.Options);
        }
    }
}
