using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Flight.Data
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<FlightDbContext>
    {
        public FlightDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<FlightDbContext>();

            builder.UseSqlServer("Server=localhost;Database=FlightDB;User ID=sa;Password=@Aa123456;TrustServerCertificate=True");
            return new FlightDbContext(builder.Options, null);
        }
    }
}
