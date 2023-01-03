using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Identity.Data;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<IdentityContext>
{
    public IdentityContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<IdentityContext>();

        builder.UseSqlServer("Server=localhost;Database=IdentityDB;User ID=sa;Password=@Aa123456;TrustServerCertificate=True");
        return new IdentityContext(builder.Options, null);
    }
}
