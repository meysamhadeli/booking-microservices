using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Passenger.Data;

public class DesignTimeDbContextFactory: IDesignTimeDbContextFactory<PassengerDbContext>
{
    public PassengerDbContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<PassengerDbContext>();

        builder.UseSqlServer("Server=localhost;Database=PassengerDB;User ID=sa;Password=@Aa123456;TrustServerCertificate=True");
        return new PassengerDbContext(builder.Options, null);
    }
}
