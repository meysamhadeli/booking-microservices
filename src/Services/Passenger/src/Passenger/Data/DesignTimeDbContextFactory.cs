using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Passenger.Data;

public class DesignTimeDbContextFactory: IDesignTimeDbContextFactory<PassengerDbContext>
{
    public PassengerDbContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<PassengerDbContext>();

        builder.UseNpgsql("Server=localhost;Port=5432;Database=passenger;User Id=postgres;Password=postgres;Include Error Detail=true")
            .UseSnakeCaseNamingConvention();
        return new PassengerDbContext(builder.Options, null);
    }
}
