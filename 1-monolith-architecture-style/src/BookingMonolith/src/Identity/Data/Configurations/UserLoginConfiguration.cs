using BookingMonolith.Identity.Identities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookingMonolith.Identity.Data.Configurations;

[RegisterIdentityConfiguration]
public class UserLoginConfiguration : IEntityTypeConfiguration<UserLogin>
{
    public void Configure(EntityTypeBuilder<UserLogin> builder)
    {
        builder.ToTable(nameof(UserLogin));

        builder.HasKey(r => new { r.LoginProvider, r.ProviderKey });

        // // ref: https://learn.microsoft.com/en-us/ef/core/saving/concurrency?tabs=fluent-api
        builder.Property(r => r.Version).IsConcurrencyToken();
    }
}

