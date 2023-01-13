using BuildingBlocks.EFCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Passenger.Data.Configurations;

public class PassengerConfiguration: IEntityTypeConfiguration<Passengers.Models.Passenger>
{
    public void Configure(EntityTypeBuilder<Passengers.Models.Passenger> builder)
    {
        builder.ToTable(nameof(Passenger));

        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id).ValueGeneratedNever();
    }
}
