using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nutrisense.Nutrisense.Platform.Subscriptions.Domain.Model.Aggregates;

namespace Nutrisense.Nutrisense.Platform.Subscriptions.Infrastructure.Persistence.EFC.Configuration;

public class UserSubscriptionEntityTypeConfiguration : IEntityTypeConfiguration<UserSubscription>
{
    public void Configure(EntityTypeBuilder<UserSubscription> builder)
    {
        builder.ToTable("user_subscriptions");
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.Property(s => s.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        builder.Property(s => s.PlanId)
            .HasColumnName("plan_id")
            .IsRequired();

        builder.Property(s => s.PlanKey)
            .HasColumnName("plan_key")
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(s => s.Status)
            .HasColumnName("status")
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(s => s.BillingPeriod)
            .HasColumnName("billing_period")
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(s => s.PeriodStart)
            .HasColumnName("period_start")
            .IsRequired();

        builder.Property(s => s.PeriodEnd)
            .HasColumnName("period_end")
            .IsRequired();

        builder.Property(s => s.CancelAtPeriodEnd)
            .HasColumnName("cancel_at_period_end")
            .IsRequired();

        builder.Property(s => s.StripeSubscriptionId)
            .HasColumnName("stripe_subscription_id")
            .IsRequired(false);

        builder.Property(s => s.StripeCustomerId)
            .HasColumnName("stripe_customer_id")
            .IsRequired(false);

        builder.Property(s => s.LastPlanChangeAt)
            .HasColumnName("last_plan_change_at")
            .IsRequired(false);

        builder.Property(s => s.PaymentMethodId)
            .HasColumnName("payment_method_id")
            .IsRequired(false);

        builder.Property(s => s.CreatedAt).HasColumnName("created_at");
        builder.Property(s => s.UpdatedAt).HasColumnName("updated_at");

        builder.HasMany(s => s.PaymentRecords)
            .WithOne()
            .HasForeignKey(r => r.UserSubscriptionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(nameof(UserSubscription.PaymentRecords))
            .HasField("_paymentRecords")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
