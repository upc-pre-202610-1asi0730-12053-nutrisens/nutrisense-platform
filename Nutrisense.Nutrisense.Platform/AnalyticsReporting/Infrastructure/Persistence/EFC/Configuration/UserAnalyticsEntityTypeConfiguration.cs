using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Nutrisense.Nutrisense.Platform.AnalyticsReporting.Domain.Model.Aggregates;

namespace Nutrisense.Nutrisense.Platform.AnalyticsReporting.Infrastructure.Persistence.EFC.Configuration;

/// <summary>EF Core mapping for the user analytics aggregate and its persisted columns.</summary>
public class UserAnalyticsEntityTypeConfiguration : IEntityTypeConfiguration<UserAnalytics>
{
    public void Configure(EntityTypeBuilder<UserAnalytics> builder)
    {
        builder.HasKey(u => u.Id);

        builder.Property(u => u.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.Property(u => u.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        builder.Property(u => u.CurrentStreak)
            .HasColumnName("current_streak")
            .IsRequired();

        builder.Property(u => u.LongestStreak)
            .HasColumnName("longest_streak")
            .IsRequired();

        // DateOnly? stored as varchar(10) — safe across all MySQL EF providers.
        builder.Property(u => u.LastLogDate)
            .HasConversion(new ValueConverter<DateOnly?, string?>(
                v => v.HasValue ? v.Value.ToString("yyyy-MM-dd") : null,
                v => v != null ? DateOnly.ParseExact(v, "yyyy-MM-dd", null) : (DateOnly?)null))
            .HasColumnName("last_log_date")
            .HasMaxLength(10);

        builder.Property(u => u.WeeklyCompletionRate)
            .HasColumnName("weekly_completion_rate")
            .HasColumnType("decimal(5,4)")
            .IsRequired();

        builder.Property(u => u.LastAdherenceScore)
            .HasColumnName("last_adherence_score")
            .HasColumnType("decimal(5,4)");

        builder.Property(u => u.LastProgressCalculatedAt)
            .HasColumnName("last_progress_calculated_at");

        builder.Property(u => u.CreatedAt).HasColumnName("created_at");
        builder.Property(u => u.UpdatedAt).HasColumnName("updated_at");

        builder.HasIndex(u => u.UserId)
            .IsUnique()
            .HasDatabaseName("ix_user_analytics_records_user_id");

        builder.HasMany(u => u.ProgressSnapshots)
            .WithOne()
            .HasForeignKey(s => s.UserAnalyticsId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
