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

        // // ref: https://learn.microsoft.com/en-us/ef/core/saving/concurrency?tabs=fluent-api
        builder.Property(r => r.Version).IsConcurrencyToken();
    }
}
