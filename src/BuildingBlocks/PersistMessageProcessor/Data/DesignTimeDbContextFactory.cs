using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace BuildingBlocks.PersistMessageProcessor.Data;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<PersistMessageDbContext>
{
    public PersistMessageDbContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<PersistMessageDbContext>();

        builder.UseSqlServer("Server=localhost;Database=PersistMessageDB;User ID=sa;Password=@Aa123456;TrustServerCertificate=True");
        return new PersistMessageDbContext(builder.Options);
    }
}
