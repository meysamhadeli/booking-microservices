using System.Reflection;
using BuildingBlocks.EFCore;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Booking.Data;

public class BookingDbContext : AppDbContextBase
{
    public const string DefaultSchema = "dbo";

    public BookingDbContext(DbContextOptions<BookingDbContext> options, IHttpContextAccessor httpContextAccessor) : base(options, httpContextAccessor)
    {
    }

    public DbSet<Booking.Models.Booking> Bookings => Set<Booking.Models.Booking>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(builder);
    }
}
