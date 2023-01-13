using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BuildingBlocks.PersistMessageProcessor.Data.Configurations;

public class PersistMessageConfiguration : IEntityTypeConfiguration<PersistMessage>
{
    public void Configure(EntityTypeBuilder<PersistMessage> builder)
    {
        builder.ToTable(nameof(PersistMessage));

        builder.HasKey(x => x.Id);

        builder.Property(r => r.Id)
            .IsRequired().ValueGeneratedNever();

        builder.Property(x => x.DeliveryType)
            .HasDefaultValue(MessageDeliveryType.Outbox)
            .HasConversion(
                x => x.ToString(),
                x => (MessageDeliveryType)Enum.Parse(typeof(MessageDeliveryType), x));


        builder.Property(x => x.MessageStatus)
            .HasDefaultValue(MessageStatus.InProgress)
            .HasConversion(
                v => v.ToString(),
                v => (MessageStatus)Enum.Parse(typeof(MessageStatus), v));
    }
}
