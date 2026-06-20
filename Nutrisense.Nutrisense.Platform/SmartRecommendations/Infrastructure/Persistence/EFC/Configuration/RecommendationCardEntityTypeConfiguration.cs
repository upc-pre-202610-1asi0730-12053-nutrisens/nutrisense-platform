using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nutrisense.Nutrisense.Platform.SmartRecommendations.Domain.Model.Aggregates;

namespace Nutrisense.Nutrisense.Platform.SmartRecommendations.Infrastructure.Persistence.EFC.Configuration;

public class RecommendationCardEntityTypeConfiguration : IEntityTypeConfiguration<RecommendationCard>
{
    public void Configure(EntityTypeBuilder<RecommendationCard> builder)
    {
        builder.HasKey(rc => rc.Id);

        builder.Property(rc => rc.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.Property(rc => rc.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        builder.HasIndex(rc => rc.UserId)
            .HasDatabaseName("ix_recommendation_cards_user_id");

        builder.Property(rc => rc.CityId)
            .HasColumnName("city_id")
            .IsRequired(false);

        builder.Property(rc => rc.WeatherType)
            .HasColumnName("weather_type")
            .HasMaxLength(20)
            .IsRequired(false);

        builder.Property(rc => rc.GoalType)
            .HasColumnName("goal_type")
            .HasMaxLength(50)
            .IsRequired(false);

        builder.Property(rc => rc.FoodId)
            .HasColumnName("food_id")
            .IsRequired(false);

        builder.Property(rc => rc.FoodNameEn)
            .HasColumnName("food_name_en")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(rc => rc.FoodNameEs)
            .HasColumnName("food_name_es")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(rc => rc.EstimatedCalories)
            .HasColumnName("estimated_calories")
            .HasColumnType("decimal(10,2)")
            .IsRequired();

        builder.Property(rc => rc.EstimatedProteinG)
            .HasColumnName("estimated_protein_g")
            .HasColumnType("decimal(10,2)")
            .IsRequired();

        builder.Property(rc => rc.EstimatedCarbsG)
            .HasColumnName("estimated_carbs_g")
            .HasColumnType("decimal(10,2)")
            .IsRequired();

        builder.Property(rc => rc.EstimatedFatG)
            .HasColumnName("estimated_fat_g")
            .HasColumnType("decimal(10,2)")
            .IsRequired();

        builder.Property(rc => rc.Badge)
            .HasColumnName("badge")
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(rc => rc.ContextLabelEn)
            .HasColumnName("context_label_en")
            .HasMaxLength(300)
            .IsRequired();

        builder.Property(rc => rc.ContextLabelEs)
            .HasColumnName("context_label_es")
            .HasMaxLength(300)
            .IsRequired();

        builder.Property(rc => rc.RestrictionsConflict)
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

        builder.Property(rc => rc.IsActive)
            .HasColumnName("is_active")
            .IsRequired();

        builder.Property(rc => rc.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();
    }
}
