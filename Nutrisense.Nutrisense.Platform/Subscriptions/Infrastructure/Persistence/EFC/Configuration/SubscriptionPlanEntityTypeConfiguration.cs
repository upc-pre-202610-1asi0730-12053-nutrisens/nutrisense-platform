using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nutrisense.Nutrisense.Platform.Subscriptions.Domain.Model.Aggregates;

namespace Nutrisense.Nutrisense.Platform.Subscriptions.Infrastructure.Persistence.EFC.Configuration;

public class SubscriptionPlanEntityTypeConfiguration : IEntityTypeConfiguration<SubscriptionPlan>
{
    public void Configure(EntityTypeBuilder<SubscriptionPlan> builder)
    {
        builder.ToTable("subscription_plans");
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.Property(p => p.Key)
            .HasColumnName("key")
            .HasMaxLength(20)
            .IsRequired();

        builder.HasIndex(p => p.Key)
            .IsUnique()
            .HasDatabaseName("ix_subscription_plans_key");

        builder.Property(p => p.PriceMonthly)
            .HasColumnName("price_monthly")
            .HasColumnType("decimal(10,2)")
            .IsRequired();

        builder.Property(p => p.PriceAnnual)
            .HasColumnName("price_annual")
            .HasColumnType("decimal(10,2)")
            .IsRequired(false);

        builder.Property(p => p.Currency)
            .HasColumnName("currency")
            .HasMaxLength(3)
            .IsRequired();

        builder.Property(p => p.Features)
            .HasColumnName("features")
            .HasConversion(
                v => System.Text.Json.JsonSerializer.Serialize(v ?? new List<string>(), (System.Text.Json.JsonSerializerOptions?)null),
                v => string.IsNullOrEmpty(v)
                    ? new List<string>()
                    : System.Text.Json.JsonSerializer.Deserialize<List<string>>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? new List<string>(),
                new ValueComparer<List<string>>(
                    (a, b) => a != null && b != null && a.SequenceEqual(b),
                    c => c.Aggregate(0, (h, e) => HashCode.Combine(h, e.GetHashCode())),
                    c => c.ToList()))
            .HasColumnType("json")
            .IsRequired();
    }
}
