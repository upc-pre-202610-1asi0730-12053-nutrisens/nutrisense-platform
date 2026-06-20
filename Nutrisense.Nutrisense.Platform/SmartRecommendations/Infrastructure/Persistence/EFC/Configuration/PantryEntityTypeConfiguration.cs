using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nutrisense.Nutrisense.Platform.SmartRecommendations.Domain.Model.Aggregates;

namespace Nutrisense.Nutrisense.Platform.SmartRecommendations.Infrastructure.Persistence.EFC.Configuration;

public class PantryEntityTypeConfiguration : IEntityTypeConfiguration<Pantry>
{
    public void Configure(EntityTypeBuilder<Pantry> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.Property(p => p.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        builder.HasIndex(p => p.UserId)
            .IsUnique()
            .HasDatabaseName("ix_pantries_user_id");

        builder.Property(p => p.CreatedAt).HasColumnName("created_at");
        builder.Property(p => p.UpdatedAt).HasColumnName("updated_at");

        builder.HasMany(p => p.Items)
            .WithOne()
            .HasForeignKey(i => i.PantryId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
