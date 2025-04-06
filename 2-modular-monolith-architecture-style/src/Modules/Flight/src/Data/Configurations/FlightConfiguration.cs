using Flight.Aircrafts.Models;
using Flight.Airports.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Flight.Data.Configurations;

using System;
using Flights.Models;
using Flights.ValueObjects;

public class FlightConfiguration : IEntityTypeConfiguration<Flights.Models.Flight>
{
    public void Configure(EntityTypeBuilder<Flights.Models.Flight> builder)
    {
        builder.ToTable(nameof(Flight));

        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id).ValueGeneratedNever()
            .HasConversion<Guid>(flight => flight.Value, dbId => FlightId.Of(dbId));

        builder.Property(r => r.Version).IsConcurrencyToken();


        builder.OwnsOne(
            x => x.FlightNumber,
            a =>
            {
                a.Property(p => p.Value)
                    .HasColumnName(nameof(Flight.FlightNumber))
                    .HasMaxLength(50)
                    .IsRequired();
            }
        );

        builder
            .HasOne<Aircraft>()
            .WithMany()
            .HasForeignKey(p => p.AircraftId)
            .IsRequired();

        builder
            .HasOne<Airport>()
            .WithMany()
            .HasForeignKey(d => d.DepartureAirportId)
            .IsRequired();

        builder
            .HasOne<Airport>()
            .WithMany()
            .HasForeignKey(d => d.ArriveAirportId)
            .IsRequired();


        builder.OwnsOne(
            x => x.DurationMinutes,
            a =>
            {
                a.Property(p => p.Value)
                    .HasColumnName(nameof(Flight.DurationMinutes))
                    .HasMaxLength(50)
                    .IsRequired();
            }
        );

        builder.Property(x => x.Status)
            .HasDefaultValue(Flights.Enums.FlightStatus.Unknown)
            .HasConversion(
                x => x.ToString(),
                x => (Flights.Enums.FlightStatus)Enum.Parse(typeof(Flights.Enums.FlightStatus), x));

        builder.OwnsOne(
            x => x.Price,
            a =>
            {
                a.Property(p => p.Value)
                    .HasColumnName(nameof(Flight.Price))
                    .HasMaxLength(10)
                    .IsRequired();
            }
        );

        builder.OwnsOne(
            x => x.ArriveDate,
            a =>
            {
                a.Property(p => p.Value)
                    .HasColumnName(nameof(Flight.ArriveDate))
                    .IsRequired();
            }
        );

        builder.OwnsOne(
            x => x.DepartureDate,
            a =>
            {
                a.Property(p => p.Value)
                    .HasColumnName(nameof(Flight.DepartureDate))
                    .IsRequired();
            }
        );

        builder.OwnsOne(
            x => x.FlightDate,
            a =>
            {
                a.Property(p => p.Value)
                    .HasColumnName(nameof(Flight.FlightDate))
                    .IsRequired();
            }
        );
    }
}
