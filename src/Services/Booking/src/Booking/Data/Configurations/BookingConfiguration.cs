using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Booking.Data.Configurations;

public class BookingConfiguration : IEntityTypeConfiguration<Booking.Models.Booking>
{
    public void Configure(EntityTypeBuilder<Booking.Models.Booking> builder)
    {
        builder.ToTable("Booking", BookingDbContext.DefaultSchema);

        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id).ValueGeneratedNever();

        builder.OwnsOne(c => c.Trip, x =>
        {
            x.Property(c => c.Description);
            x.Property(c => c.Price);
            x.Property(c => c.AircraftId);
            x.Property(c => c.FlightDate);
            x.Property(c => c.FlightNumber);
            x.Property(c => c.SeatNumber);
            x.Property(c => c.ArriveAirportId);
            x.Property(c => c.DepartureAirportId);
        });

        builder.OwnsOne(c => c.PassengerInfo, x =>
        {
            x.Property(c => c.Name);
        });
    }
}
