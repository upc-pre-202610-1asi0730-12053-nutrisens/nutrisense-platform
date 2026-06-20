using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nutrisense.Nutrisense.Platform.SmartRecommendations.Domain.Model.Aggregates;

namespace Nutrisense.Nutrisense.Platform.SmartRecommendations.Infrastructure.Persistence.EFC.Configuration;

public class RecipeEntityTypeConfiguration : IEntityTypeConfiguration<Recipe>
{
    public void Configure(EntityTypeBuilder<Recipe> builder)
    {
        builder.HasKey(r => r.Id);

        builder.Property(r => r.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.Property(r => r.Key)
            .HasColumnName("key")
            .HasMaxLength(100)
            .IsRequired();

        builder.HasIndex(r => r.Key)
            .IsUnique()
            .HasDatabaseName("ix_recipes_key");

        builder.Property(r => r.NameEn)
            .HasColumnName("name_en")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(r => r.NameEs)
            .HasColumnName("name_es")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(r => r.GoalType)
            .HasColumnName("goal_type")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(r => r.PrepTimeMinutes)
            .HasColumnName("prep_time_minutes")
            .IsRequired();

        builder.Property(r => r.Servings)
            .HasColumnName("servings")
            .IsRequired();

        builder.Property(r => r.TotalCalories)
            .HasColumnName("total_calories")
            .HasColumnType("decimal(10,2)")
            .IsRequired();

        builder.Property(r => r.TotalProteinG)
            .HasColumnName("total_protein_g")
            .HasColumnType("decimal(10,2)")
            .IsRequired();

        builder.Property(r => r.TotalCarbsG)
            .HasColumnName("total_carbs_g")
            .HasColumnType("decimal(10,2)")
            .IsRequired();

        builder.Property(r => r.TotalFatG)
            .HasColumnName("total_fat_g")
            .HasColumnType("decimal(10,2)")
            .IsRequired();

        builder.Property(r => r.TotalFiberG)
            .HasColumnName("total_fiber_g")
            .HasColumnType("decimal(10,2)")
            .IsRequired();

        builder.Property(r => r.RestrictionsConflict)
            .HasConversion(
                v => System.Text.Json.JsonSerializer.Serialize(v ?? new List<string>(), (System.Text.Json.JsonSerializerOptions?)null),
                v => string.IsNullOrEmpty(v)
                    ? new List<string>()
                    : System.Text.Json.JsonSerializer.Deserialize<List<string>>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? new List<string>(),
                new ValueComparer<List<string>>(
                    (a, b) => a != null && b != null && a.SequenceEqual(b),
                    c => c.Aggregate(0, (h, e) => HashCode.Combine(h, e.GetHashCode())),
                    c => c.ToList()))
            .HasColumnName("restrictions_conflict")
            .HasColumnType("json")
            .IsRequired();

        builder.HasMany(r => r.Ingredients)
            .WithOne()
            .HasForeignKey(ri => ri.RecipeId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
