using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nutrisense.Nutrisense.Platform.SmartRecommendations.Domain.Model.Aggregates;

namespace Nutrisense.Nutrisense.Platform.SmartRecommendations.Infrastructure.Persistence.EFC.Configuration;

public class IngredientCatalogItemEntityTypeConfiguration : IEntityTypeConfiguration<IngredientCatalogItem>
{
    public void Configure(EntityTypeBuilder<IngredientCatalogItem> builder)
    {
        builder.HasKey(i => i.Id);

        builder.Property(i => i.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.Property(i => i.Key)
            .HasColumnName("key")
            .HasMaxLength(100)
            .IsRequired();

        builder.HasIndex(i => i.Key)
            .IsUnique()
            .HasDatabaseName("ix_ingredient_catalog_items_key");

        builder.Property(i => i.NameEn)
            .HasColumnName("name_en")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(i => i.NameEs)
            .HasColumnName("name_es")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(i => i.FoodId)
            .HasColumnName("food_id")
            .IsRequired(false);

        builder.Property(i => i.Category)
            .HasColumnName("category")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(i => i.DefaultUnit)
            .HasColumnName("default_unit")
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(i => i.GramsPerUnit)
            .HasColumnName("grams_per_unit")
            .HasColumnType("decimal(10,3)")
            .IsRequired(false);
    }
}
