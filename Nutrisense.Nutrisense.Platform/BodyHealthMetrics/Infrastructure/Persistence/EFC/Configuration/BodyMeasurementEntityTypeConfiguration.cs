using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nutrisense.Nutrisense.Platform.BodyHealthMetrics.Domain.Model.Entities;
using Nutrisense.Nutrisense.Platform.BodyHealthMetrics.Domain.Model.ValueObjects;

namespace Nutrisense.Nutrisense.Platform.BodyHealthMetrics.Infrastructure.Persistence.EFC.Configuration;

/// <summary>EF Core table mapping for the BodyMeasurement entity.</summary>
public class BodyMeasurementEntityTypeConfiguration : IEntityTypeConfiguration<BodyMeasurement>
{
    public void Configure(EntityTypeBuilder<BodyMeasurement> builder)
    {
        builder.ToTable("body_measurements");
        builder.HasKey(bm => bm.Id);

        builder.Property(bm => bm.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.Property(bm => bm.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        builder.Property(bm => bm.WaistCm)
            .HasConversion(
                w => w.Cm,
                v => new WaistMeasurement(v))
            .HasColumnName("waist_cm")
            .HasColumnType("decimal(5,2)")
            .IsRequired();

        builder.Property(bm => bm.NeckCm)
            .HasConversion(
                n => n.Cm,
                v => new NeckMeasurement(v))
            .HasColumnName("neck_cm")
            .HasColumnType("decimal(5,2)")
            .IsRequired();

        builder.Property(bm => bm.MeasuredAt)
            .HasColumnName("measured_at")
            .IsRequired();
    }
}
