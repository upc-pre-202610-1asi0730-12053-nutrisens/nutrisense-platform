using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Nutrisense.Nutrisense.Platform.NutritionTracking.Domain.Model.Aggregates;

namespace Nutrisense.Nutrisense.Platform.NutritionTracking.Infrastructure.Persistence.EFC.Configuration;

public class NutritionLogEntityTypeConfiguration : IEntityTypeConfiguration<NutritionLog>
{
    public void Configure(EntityTypeBuilder<NutritionLog> builder)
    {
        builder.HasKey(n => n.Id);

        builder.Property(n => n.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.Property(n => n.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        builder.Property(n => n.FoodId)
            .HasColumnName("food_id")
            .IsRequired();

        builder.Property(n => n.FoodNameEn)
            .HasColumnName("food_name_en")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(n => n.FoodNameEs)
            .HasColumnName("food_name_es")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(n => n.MealType)
            .HasColumnName("meal_type")
            .HasMaxLength(20)
            .IsRequired();

        // DateOnly stored as varchar(10) with yyyy-MM-dd format — safe across all MySQL EF providers.
        builder.Property(n => n.Date)
            .HasConversion(new ValueConverter<DateOnly, string>(
                d => d.ToString("yyyy-MM-dd"),
                s => DateOnly.ParseExact(s, "yyyy-MM-dd", null)))
            .HasColumnName("date")
            .HasMaxLength(10)
            .IsRequired();

        builder.Property(n => n.QuantityG)
            .HasColumnName("quantity_g")
            .HasColumnType("decimal(10,2)")
            .IsRequired();

        builder.Property(n => n.Calories)
            .HasColumnName("calories")
            .HasColumnType("decimal(10,2)")
            .IsRequired();

        builder.Property(n => n.ProteinG)
            .HasColumnName("protein_g")
            .HasColumnType("decimal(10,2)")
            .IsRequired();

        builder.Property(n => n.CarbsG)
            .HasColumnName("carbs_g")
            .HasColumnType("decimal(10,2)")
            .IsRequired();

        builder.Property(n => n.FatG)
            .HasColumnName("fat_g")
            .HasColumnType("decimal(10,2)")
            .IsRequired();

        builder.Property(n => n.FiberG)
            .HasColumnName("fiber_g")
            .HasColumnType("decimal(10,2)")
            .IsRequired();

        builder.Property(n => n.SugarG)
            .HasColumnName("sugar_g")
            .HasColumnType("decimal(10,2)")
            .IsRequired();

        builder.Property(n => n.Source)
            .HasColumnName("source")
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(n => n.LoggedAt)
            .HasColumnName("logged_at")
            .IsRequired();

        builder.Property(n => n.ScanType)
            .HasColumnName("scan_type")
            .HasMaxLength(20)
            .IsRequired(false);

        builder.Property(n => n.ScanConfidence)
            .HasColumnName("scan_confidence")
            .HasColumnType("decimal(5,4)")
            .IsRequired(false);

        builder.Property(n => n.ScanImageUri)
            .HasColumnName("scan_image_uri")
            .HasMaxLength(2048)
            .IsRequired(false);

        builder.Property(n => n.CreatedAt).HasColumnName("created_at");
        builder.Property(n => n.UpdatedAt).HasColumnName("updated_at");

        builder.HasIndex(n => new { n.UserId, n.Date })
            .HasDatabaseName("ix_nutrition_logs_user_date");
    }
}
