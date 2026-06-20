using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nutrisense.Nutrisense.Platform.NutritionTracking.Domain.Model.Aggregates;

namespace Nutrisense.Nutrisense.Platform.NutritionTracking.Infrastructure.Persistence.EFC.Configuration;

public class FoodEntityTypeConfiguration : IEntityTypeConfiguration<Food>
{
    public void Configure(EntityTypeBuilder<Food> builder)
    {
        builder.HasKey(f => f.Id);

        builder.Property(f => f.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.Property(f => f.Key)
            .HasColumnName("key")
            .HasMaxLength(300)
            .IsRequired();

        builder.HasIndex(f => f.Key)
            .IsUnique()
            .HasDatabaseName("ix_foods_key");

        builder.Property(f => f.Source)
            .HasColumnName("source")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(f => f.ExternalId)
            .HasColumnName("external_id")
            .HasMaxLength(200)
            .IsRequired(false);

        builder.HasIndex(f => f.ExternalId)
            .HasDatabaseName("ix_foods_external_id");

        builder.Property(f => f.NameEn)
            .HasColumnName("name_en")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(f => f.NameEs)
            .HasColumnName("name_es")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(f => f.Category)
            .HasColumnName("category")
            .HasMaxLength(50)
            .HasDefaultValue("Other")
            .IsRequired();

        builder.Property(f => f.ServingSizeG)
            .HasColumnName("serving_size_g")
            .HasColumnType("decimal(10,2)")
            .IsRequired();

        builder.Property(f => f.ServingUnit)
            .HasColumnName("serving_unit")
            .HasMaxLength(10)
            .IsRequired();

        builder.Property(f => f.CaloriesPer100g)
            .HasColumnName("calories_per100g")
            .HasColumnType("decimal(10,2)")
            .IsRequired();

        builder.Property(f => f.ProteinPer100g)
            .HasColumnName("protein_per100g")
            .HasColumnType("decimal(10,2)")
            .IsRequired();

        builder.Property(f => f.CarbsPer100g)
            .HasColumnName("carbs_per100g")
            .HasColumnType("decimal(10,2)")
            .IsRequired();

        builder.Property(f => f.FatPer100g)
            .HasColumnName("fat_per100g")
            .HasColumnType("decimal(10,2)")
            .IsRequired();

        builder.Property(f => f.FiberPer100g)
            .HasColumnName("fiber_per100g")
            .HasColumnType("decimal(10,2)")
            .IsRequired();

        builder.Property(f => f.SugarPer100g)
            .HasColumnName("sugar_per100g")
            .HasColumnType("decimal(10,2)")
            .IsRequired();

        builder.Property(f => f.Restrictions)
            .HasConversion(
                v => System.Text.Json.JsonSerializer.Serialize(v ?? new List<string>(), (System.Text.Json.JsonSerializerOptions?)null),
                v => string.IsNullOrEmpty(v)
                    ? new List<string>()
                    : System.Text.Json.JsonSerializer.Deserialize<List<string>>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? new List<string>(),
                new ValueComparer<List<string>>(
                    (a, b) => a != null && b != null && a.SequenceEqual(b),
                    c => c.Aggregate(0, (h, e) => HashCode.Combine(h, e.GetHashCode())),
                    c => c.ToList()))
            .HasColumnName("restrictions")
            .HasColumnType("json")
            .IsRequired();

        builder.Property(f => f.CreatedAt).HasColumnName("created_at");
        builder.Property(f => f.UpdatedAt).HasColumnName("updated_at");
    }
}
