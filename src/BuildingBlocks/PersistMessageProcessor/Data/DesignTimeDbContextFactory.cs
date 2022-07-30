using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace BuildingBlocks.PersistMessageProcessor.Data;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<PersistMessageDbContext>
{
    public PersistMessageDbContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<PersistMessageDbContext>();

        builder.UseSqlServer(
            "Data Source=.\\sqlexpress;Initial Catalog=PersistMessageDB;Persist Security Info=False;Integrated Security=SSPI");
        return new PersistMessageDbContext(builder.Options);
    }
}
