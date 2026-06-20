using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Nutrisense.Nutrisense.Platform.BodyHealthMetrics.Domain.Model.Aggregates;

namespace Nutrisense.Nutrisense.Platform.BodyHealthMetrics.Infrastructure.Persistence.EFC.Configuration;

/// <summary>EF Core table mapping for the BodyMetrics aggregate root.</summary>
public class BodyMetricsEntityTypeConfiguration : IEntityTypeConfiguration<BodyMetrics>
{
    public void Configure(EntityTypeBuilder<BodyMetrics> builder)
    {
        builder.ToTable("body_metrics");
        builder.HasKey(b => b.Id);

        builder.Property(b => b.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.Property(b => b.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        // One aggregate per user — used as principal key for child collection FKs
        builder.HasAlternateKey(b => b.UserId);

        builder.Property(b => b.HeightCm)
            .HasColumnName("height_cm")
            .HasColumnType("decimal(5,2)")
            .IsRequired();

        builder.Property(b => b.DateOfBirth)
            .HasConversion(new ValueConverter<DateOnly?, string?>(
                v => v.HasValue ? v.Value.ToString("yyyy-MM-dd") : null,
                s => s != null ? DateOnly.ParseExact(s, "yyyy-MM-dd", null) : (DateOnly?)null))
            .HasColumnName("date_of_birth")
            .IsRequired(false);

        builder.Property(b => b.BiologicalSex)
            .HasColumnName("biological_sex")
            .HasMaxLength(25)
            .IsRequired(false);

        builder.Property(b => b.ActivityLevel)
            .HasColumnName("activity_level")
            .HasMaxLength(25)
            .IsRequired(false);

        // BMI backing fields (BmiResult VO is a computed property — Ignore it)
        builder.Ignore(b => b.Bmi);

        builder.Property(b => b.BmiValue)
            .HasColumnName("bmi_value")
            .HasColumnType("decimal(5,2)")
            .IsRequired(false);

        builder.Property(b => b.BmiCategory)
            .HasColumnName("bmi_category")
            .HasMaxLength(20)
            .IsRequired(false);

        builder.Property(b => b.Bmr)
            .HasColumnName("bmr")
            .HasColumnType("decimal(8,2)")
            .IsRequired(false);

        builder.Property(b => b.Tdee)
            .HasColumnName("tdee")
            .HasColumnType("decimal(8,2)")
            .IsRequired(false);

        // MacroTargets backing fields (MacroTargets VO is a computed property — Ignore it)
        builder.Ignore(b => b.MacroTargets);

        builder.Property(b => b.MacroCalories)
            .HasColumnName("macro_daily_calories")
            .IsRequired(false);

        builder.Property(b => b.MacroProteinG)
            .HasColumnName("macro_protein_g")
            .HasColumnType("decimal(6,1)")
            .IsRequired(false);

        builder.Property(b => b.MacroCarbsG)
            .HasColumnName("macro_carbs_g")
            .HasColumnType("decimal(6,1)")
            .IsRequired(false);

        builder.Property(b => b.MacroFatG)
            .HasColumnName("macro_fat_g")
            .HasColumnType("decimal(6,1)")
            .IsRequired(false);

        builder.Property(b => b.MacroFiberG)
            .HasColumnName("macro_fiber_g")
            .HasColumnType("decimal(6,1)")
            .IsRequired(false);

        builder.Property(b => b.CreatedAt).HasColumnName("created_at");
        builder.Property(b => b.UpdatedAt).HasColumnName("updated_at");

        // Child collections FK via body_metrics.user_id (alternate key — cross-BC, no FK to users)
        builder.HasMany(b => b.WeightLogs)
            .WithOne()
            .HasForeignKey(wl => wl.UserId)
            .HasPrincipalKey(b => b.UserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(b => b.BodyMeasurements)
            .WithOne()
            .HasForeignKey(bm => bm.UserId)
            .HasPrincipalKey(b => b.UserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(b => b.UserGoals)
            .WithOne()
            .HasForeignKey(g => g.UserId)
            .HasPrincipalKey(b => b.UserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}
