using Flight.Seats.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Flight.Data.Configurations;

using System;
using Seats.ValueObjects;

public class SeatConfiguration : IEntityTypeConfiguration<Seat>
{
    public void Configure(EntityTypeBuilder<Seat> builder)
    {
        builder.ToTable(nameof(Seat));

        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id).ValueGeneratedNever()
            .HasConversion<Guid>(seatId => seatId.Value, dbId => SeatId.Of(dbId));

        builder.Property(r => r.Version).IsConcurrencyToken();

        builder.OwnsOne(
            x => x.SeatNumber,
            a =>
            {
                a.Property(p => p.Value)
                    .HasColumnName(nameof(Seat.SeatNumber))
                    .HasMaxLength(50)
                    .IsRequired();
            }
        );

        builder
            .HasOne<Flights.Models.Flight>()
            .WithMany()
            .HasForeignKey(p => p.FlightId);

        builder.Property(x => x.Class)
            .HasDefaultValue(Seats.Enums.SeatClass.Unknown)
            .HasConversion(
                x => x.ToString(),
                x => (Flight.Seats.Enums.SeatClass)Enum.Parse(typeof(Flight.Seats.Enums.SeatClass), x));

        builder.Property(x => x.Type)
            .HasDefaultValue(Seats.Enums.SeatType.Unknown)
            .HasConversion(
                x => x.ToString(),
                x => (Flight.Seats.Enums.SeatType)Enum.Parse(typeof(Flight.Seats.Enums.SeatType), x));
    }
}
