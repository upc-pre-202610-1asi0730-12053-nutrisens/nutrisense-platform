using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nutrisense.Nutrisense.Platform.Subscriptions.Domain.Model.Aggregates;

namespace Nutrisense.Nutrisense.Platform.Subscriptions.Infrastructure.Persistence.EFC.Configuration;

public class PaymentMethodEntityTypeConfiguration : IEntityTypeConfiguration<PaymentMethod>
{
    public void Configure(EntityTypeBuilder<PaymentMethod> builder)
    {
        builder.ToTable("payment_methods");
        builder.HasKey(m => m.Id);

        builder.Property(m => m.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.Property(m => m.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        builder.Property(m => m.LastFour)
            .HasColumnName("last_four")
            .HasMaxLength(4)
            .IsRequired();

        builder.Property(m => m.Brand)
            .HasColumnName("brand")
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(m => m.ExpiryMonth)
            .HasColumnName("expiry_month")
            .IsRequired();

        builder.Property(m => m.ExpiryYear)
            .HasColumnName("expiry_year")
            .IsRequired();

        builder.Property(m => m.CardholderName)
            .HasColumnName("cardholder_name")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(m => m.StripePaymentMethodId)
            .HasColumnName("stripe_payment_method_id")
            .IsRequired();

        builder.Property(m => m.CreatedAt).HasColumnName("created_at");
        builder.Property(m => m.UpdatedAt).HasColumnName("updated_at");
    }
}
