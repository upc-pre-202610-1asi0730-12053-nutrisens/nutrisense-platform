using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nutrisense.Nutrisense.Platform.BodyHealthMetrics.Domain.Model.Entities;
using Nutrisense.Nutrisense.Platform.BodyHealthMetrics.Domain.Model.ValueObjects;

namespace Nutrisense.Nutrisense.Platform.BodyHealthMetrics.Infrastructure.Persistence.EFC.Configuration;

/// <summary>EF Core table mapping for the WeightLog entity.</summary>
public class WeightLogEntityTypeConfiguration : IEntityTypeConfiguration<WeightLog>
{
    public void Configure(EntityTypeBuilder<WeightLog> builder)
    {
        builder.ToTable("weight_logs");
        builder.HasKey(wl => wl.Id);

        builder.Property(wl => wl.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.Property(wl => wl.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        builder.Property(wl => wl.WeightKg)
            .HasConversion(
                w => w.Value,
                v => new WeightKg(v))
            .HasColumnName("weight_kg")
            .HasColumnType("decimal(5,2)")
            .IsRequired();

        builder.Property(wl => wl.LoggedAt)
            .HasColumnName("logged_at")
            .IsRequired();

        builder.Property(wl => wl.Note)
            .HasColumnName("note")
            .HasMaxLength(500)
            .IsRequired(false);
    }
}
