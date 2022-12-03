using BuildingBlocks.EFCore;
using Flight.Seats.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Flight.Data.Configurations;

public class SeatConfiguration : IEntityTypeConfiguration<Seat>
{
    public void Configure(EntityTypeBuilder<Seat> builder)
    {
        builder.ToTable("Seat", AppDbContextBase.DefaultSchema);

        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id).ValueGeneratedNever();

        builder
            .HasOne<Flights.Models.Flight>()
            .WithMany()
            .HasForeignKey(p => p.FlightId);
    }
}
