using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Flight.Data
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<FlightDbContext>
    {
        public FlightDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<FlightDbContext>();

            builder.UseSqlServer(
                "Data Source=.\\sqlexpress;Initial Catalog=FlightDB;Persist Security Info=False;Integrated Security=SSPI");
            return new FlightDbContext(builder.Options, null);
        }
    }
}
