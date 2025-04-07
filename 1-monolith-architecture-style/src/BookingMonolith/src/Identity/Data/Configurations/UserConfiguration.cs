using BookingMonolith.Identity.Identities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookingMonolith.Identity.Data.Configurations;

[RegisterIdentityConfiguration]
public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable(nameof(User));

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Id)
            .IsRequired()
            .ValueGeneratedOnAdd(); // Auto-generate GUID on add

        // // ref: https://learn.microsoft.com/en-us/ef/core/saving/concurrency?tabs=fluent-api
        builder.Property(r => r.Version).IsConcurrencyToken();
    }
}
