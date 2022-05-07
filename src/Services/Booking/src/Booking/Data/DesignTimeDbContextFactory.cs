using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Booking.Data;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<BookingDbContext>
{
    public BookingDbContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<BookingDbContext>();

        builder.UseSqlServer(
            "Data Source=.\\sqlexpress;Initial Catalog=BookingDB;Persist Security Info=False;Integrated Security=SSPI");
        return new BookingDbContext(builder.Options, null);
    }
}
