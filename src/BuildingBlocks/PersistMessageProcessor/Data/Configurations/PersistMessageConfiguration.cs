using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BuildingBlocks.PersistMessageProcessor.Data.Configurations;

public class PersistMessageConfiguration : IEntityTypeConfiguration<PersistMessage>
{
    public void Configure(EntityTypeBuilder<PersistMessage> builder)
    {
        builder.ToTable("PersistMessage", PersistMessageDbContext.DefaultSchema);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .IsRequired();

        builder.Property(x => x.DeliveryType)
            .HasMaxLength(50)
            .HasConversion(
                v => v.ToString(),
                v => (MessageDeliveryType)Enum.Parse(typeof(MessageDeliveryType), v))
            .IsRequired()
            .IsUnicode(false);

        builder.Property(x => x.DeliveryType)
            .HasMaxLength(50)
            .HasConversion(
                v => v.ToString(),
                v => (MessageDeliveryType)Enum.Parse(typeof(MessageDeliveryType), v))
            .IsRequired()
            .IsUnicode(false);

        builder.Property(x => x.MessageStatus)
            .HasMaxLength(50)
            .HasConversion(
                v => v.ToString(),
                v => (MessageStatus)Enum.Parse(typeof(MessageStatus), v))
            .IsRequired()
            .IsUnicode(false);
    }
}
