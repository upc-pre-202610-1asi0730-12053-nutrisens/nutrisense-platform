using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nutrisense.Nutrisense.Platform.SmartRecommendations.Domain.Model.Entities;

namespace Nutrisense.Nutrisense.Platform.SmartRecommendations.Infrastructure.Persistence.EFC.Configuration;

public class RecipeIngredientItemEntityTypeConfiguration : IEntityTypeConfiguration<RecipeIngredientItem>
{
    public void Configure(EntityTypeBuilder<RecipeIngredientItem> builder)
    {
        builder.HasKey(ri => ri.Id);

        builder.Property(ri => ri.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.Property(ri => ri.RecipeId)
            .HasColumnName("recipe_id")
            .IsRequired();

        builder.Property(ri => ri.IngredientCatalogItemId)
            .HasColumnName("ingredient_catalog_item_id")
            .IsRequired();

        builder.Property(ri => ri.Quantity)
            .HasColumnName("quantity")
            .HasColumnType("decimal(10,3)")
            .IsRequired();

        builder.Property(ri => ri.Unit)
            .HasColumnName("unit")
            .HasMaxLength(20)
            .IsRequired();
    }
}
