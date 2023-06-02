using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Passenger.Data.Configurations;

using Passengers.ValueObjects;

public class PassengerConfiguration : IEntityTypeConfiguration<Passengers.Models.Passenger>
{
    public void Configure(EntityTypeBuilder<Passengers.Models.Passenger> builder)
    {
        builder.ToTable(nameof(Passengers.Models.Passenger));

        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id).ValueGeneratedNever()
            .HasConversion<Guid>(passengerId => passengerId.Value, dbId => PassengerId.Of(dbId));

        builder.Property(r => r.Version).IsConcurrencyToken();

        builder.OwnsOne(
            x => x.Name,
            a =>
            {
                a.Property(p => p.Value)
                    .HasColumnName(nameof(Passengers.Models.Passenger.Name))
                    .HasMaxLength(50)
                    .IsRequired();
            }
        );

        builder.OwnsOne(
            x => x.PassportNumber,
            a =>
            {
                a.Property(p => p.Value)
                    .HasColumnName(nameof(Passengers.Models.Passenger.PassportNumber))
                    .HasMaxLength(10)
                    .IsRequired();
            }
        );

        builder.OwnsOne(
            x => x.Age,
            a =>
            {
                a.Property(p => p.Value)
                    .HasColumnName(nameof(Passengers.Models.Passenger.Age))
                    .HasMaxLength(3)
                    .IsRequired();
            }
        );

        builder.Property(x => x.PassengerType)
            .IsRequired()
            .HasDefaultValue(Passengers.Enums.PassengerType.Unknown)
            .HasConversion(
                x => x.ToString(),
                x => (Passengers.Enums.PassengerType)Enum.Parse(typeof(Passengers.Enums.PassengerType), x));
    }
}
