using Flight.Aircrafts.Models;
using Flight.Airports.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Flight.Data.Configurations;

public class FlightConfiguration : IEntityTypeConfiguration<Flights.Models.Flight>
{
    public void Configure(EntityTypeBuilder<Flights.Models.Flight> builder)
    {
        builder.ToTable("Flight", "dbo");

        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id).ValueGeneratedNever();

        builder
            .HasOne<Aircraft>()
            .WithMany()
            .HasForeignKey(p => p.AircraftId);

        builder
            .HasOne<Airport>()
            .WithMany()
            .HasForeignKey(d => d.DepartureAirportId)
            .HasForeignKey(a => a.ArriveAirportId);

        // // https://docs.microsoft.com/en-us/ef/core/modeling/shadow-properties
        // // https://docs.microsoft.com/en-us/ef/core/modeling/owned-entities
        // builder.OwnsMany(p => p.Seats, a =>
        // {
        //     a.WithOwner().HasForeignKey("FlightId");
        //     a.Property<long>("Id");
        //     a.HasKey("Id");
        //     a.Property<long>("FlightId");
        //     a.ToTable("Seat");
        // });
    }
}
