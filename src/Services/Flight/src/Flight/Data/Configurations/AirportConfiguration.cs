using BuildingBlocks.EFCore;
using Flight.Airports.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Flight.Data.Configurations;

public class AirportConfiguration: IEntityTypeConfiguration<Airport>
{
    public void Configure(EntityTypeBuilder<Airport> builder)
    {
        builder.ToTable("Airport", AppDbContextBase.DefaultSchema);

        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id).ValueGeneratedNever();
    }
}
