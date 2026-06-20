using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nutrisense.Nutrisense.Platform.SmartRecommendations.Domain.Model.Entities;

namespace Nutrisense.Nutrisense.Platform.SmartRecommendations.Infrastructure.Persistence.EFC.Configuration;

public class PantryItemEntityTypeConfiguration : IEntityTypeConfiguration<PantryItem>
{
    public void Configure(EntityTypeBuilder<PantryItem> builder)
    {
        builder.HasKey(pi => pi.Id);

        builder.Property(pi => pi.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.Property(pi => pi.PantryId)
            .HasColumnName("pantry_id")
            .IsRequired();

        builder.Property(pi => pi.IngredientCatalogItemId)
            .HasColumnName("ingredient_catalog_item_id")
            .IsRequired();

        builder.Property(pi => pi.Quantity)
            .HasColumnName("quantity")
            .HasColumnType("decimal(10,3)")
            .IsRequired();

        builder.Property(pi => pi.Unit)
            .HasColumnName("unit")
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(pi => pi.ExpiresAt)
            .HasColumnName("expires_at")
            .IsRequired(false);
    }
}
