using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Nutrisense.Nutrisense.Platform.ActivityWearable.Domain.Model.Aggregates;

namespace Nutrisense.Nutrisense.Platform.ActivityWearable.Infrastructure.Persistence.EFC.Configuration;

/// <summary>EF Core mapping for the <see cref="ActivityLog"/> aggregate: keys, column names, lengths and the date converter.</summary>
public class ActivityLogEntityTypeConfiguration : IEntityTypeConfiguration<ActivityLog>
{
    /// <summary>Configures the table mapping, column conventions and the (UserId, Date) index for <see cref="ActivityLog"/>.</summary>
    /// <param name="builder">The entity type builder supplied by EF Core.</param>
    public void Configure(EntityTypeBuilder<ActivityLog> builder)
    {
        builder.HasKey(a => a.Id);

        builder.Property(a => a.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.Property(a => a.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        // DateOnly stored as varchar(10) with yyyy-MM-dd format — safe across all MySQL EF providers.
        builder.Property(a => a.Date)
            .HasConversion(new ValueConverter<DateOnly, string>(
                d => d.ToString("yyyy-MM-dd"),
                s => DateOnly.ParseExact(s, "yyyy-MM-dd", null)))
            .HasColumnName("date")
            .HasMaxLength(10)
            .IsRequired();

        builder.Property(a => a.ActivityType)
            .HasColumnName("activity_type")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(a => a.DurationMinutes)
            .HasColumnName("duration_minutes")
            .IsRequired();

        builder.Property(a => a.Intensity)
            .HasColumnName("intensity")
            .HasMaxLength(10)
            .IsRequired();

        builder.Property(a => a.CaloriesBurned)
            .HasColumnName("calories_burned")
            .HasColumnType("decimal(10,2)")
            .IsRequired();

        builder.Property(a => a.Source)
            .HasColumnName("source")
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(a => a.LoggedAt)
            .HasColumnName("logged_at")
            .IsRequired();

        builder.Property(a => a.CreatedAt).HasColumnName("created_at");
        builder.Property(a => a.UpdatedAt).HasColumnName("updated_at");

        builder.HasIndex(a => new { a.UserId, a.Date })
            .HasDatabaseName("ix_activity_logs_user_date");
    }
}
