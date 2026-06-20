using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nutrisense.Nutrisense.Platform.Subscriptions.Domain.Model.Entities;

namespace Nutrisense.Nutrisense.Platform.Subscriptions.Infrastructure.Persistence.EFC.Configuration;

public class PaymentRecordEntityTypeConfiguration : IEntityTypeConfiguration<PaymentRecord>
{
    public void Configure(EntityTypeBuilder<PaymentRecord> builder)
    {
        builder.ToTable("payment_records");
        builder.HasKey(r => r.Id);

        builder.Property(r => r.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.Property(r => r.UserSubscriptionId)
            .HasColumnName("user_subscription_id")
            .IsRequired();

        builder.Property(r => r.Amount)
            .HasColumnName("amount")
            .HasColumnType("decimal(10,2)")
            .IsRequired();

        builder.Property(r => r.Currency)
            .HasColumnName("currency")
            .HasMaxLength(3)
            .IsRequired();

        builder.Property(r => r.Status)
            .HasColumnName("status")
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(r => r.ProcessedAt)
            .HasColumnName("processed_at")
            .IsRequired();

        builder.Property(r => r.StripePaymentIntentId)
            .HasColumnName("stripe_payment_intent_id")
            .IsRequired(false);
    }
}
