using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace BuildingBlocks.PersistMessageProcessor.Data;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<PersistMessageDbContext>
{
    public PersistMessageDbContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<PersistMessageDbContext>();

        builder.UseNpgsql("Server=localhost;Port=5432;Database=persist_message;User Id=postgres;Password=postgres;Include Error Detail=true")
            .UseSnakeCaseNamingConvention();
        return new PersistMessageDbContext(builder.Options, null);
    }
}
