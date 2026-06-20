using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Nutrisense.Nutrisense.Platform.AnalyticsReporting.Domain.Model.Entities;

namespace Nutrisense.Nutrisense.Platform.AnalyticsReporting.Infrastructure.Persistence.EFC.Configuration;

/// <summary>EF Core mapping for the daily progress snapshot entity and its persisted columns.</summary>
public class ProgressSnapshotEntityTypeConfiguration : IEntityTypeConfiguration<ProgressSnapshot>
{
    public void Configure(EntityTypeBuilder<ProgressSnapshot> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.Property(s => s.UserAnalyticsId)
            .HasColumnName("user_analytics_id")
            .IsRequired();

        // DateOnly stored as varchar(10) — safe across all MySQL EF providers.
        builder.Property(s => s.Date)
            .HasConversion(new ValueConverter<DateOnly, string>(
                d => d.ToString("yyyy-MM-dd"),
                s => DateOnly.ParseExact(s, "yyyy-MM-dd", null)))
            .HasColumnName("date")
            .HasMaxLength(10)
            .IsRequired();

        builder.Property(s => s.TotalCalories)
            .HasColumnName("total_calories")
            .HasColumnType("decimal(10,2)")
            .IsRequired();

        builder.Property(s => s.TotalProteinG)
            .HasColumnName("total_protein_g")
            .HasColumnType("decimal(10,2)")
            .IsRequired();

        builder.Property(s => s.TotalCarbsG)
            .HasColumnName("total_carbs_g")
            .HasColumnType("decimal(10,2)")
            .IsRequired();

        builder.Property(s => s.TotalFatG)
            .HasColumnName("total_fat_g")
            .HasColumnType("decimal(10,2)")
            .IsRequired();

        builder.Property(s => s.AdherenceScore)
            .HasColumnName("adherence_score")
            .HasColumnType("decimal(5,4)")
            .IsRequired();

        builder.Property(s => s.IsComplete)
            .HasColumnName("is_complete")
            .IsRequired();

        builder.Property(s => s.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.HasIndex(s => new { s.UserAnalyticsId, s.Date })
            .HasDatabaseName("ix_progress_snapshots_analytics_date");
    }
}
