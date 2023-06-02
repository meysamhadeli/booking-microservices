using Flight.Aircrafts.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Flight.Data.Configurations;

using System;
using Aircrafts.ValueObjects;

public class AircraftConfiguration : IEntityTypeConfiguration<Aircraft>
{
    public void Configure(EntityTypeBuilder<Aircraft> builder)
    {

        builder.ToTable(nameof(Aircraft));

        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id).ValueGeneratedNever()
            .HasConversion<Guid>(aircraftId => aircraftId.Value, dbId => AircraftId.Of(dbId));

        builder.Property(r => r.Version).IsConcurrencyToken();

        builder.OwnsOne(
            x => x.Name,
            a =>
            {
                a.Property(p => p.Value)
                    .HasColumnName(nameof(Aircraft.Name))
                    .HasMaxLength(50)
                    .IsRequired();
            }
        );

        builder.OwnsOne(
            x => x.Model,
            a =>
            {
                a.Property(p => p.Value)
                    .HasColumnName(nameof(Aircraft.Model))
                    .HasMaxLength(50)
                    .IsRequired();
            }
        );

        builder.OwnsOne(
            x => x.ManufacturingYear,
            a =>
            {
                a.Property(p => p.Value)
                    .HasColumnName(nameof(Aircraft.ManufacturingYear))
                    .HasMaxLength(5)
                    .IsRequired();
            }
        );
    }
}
