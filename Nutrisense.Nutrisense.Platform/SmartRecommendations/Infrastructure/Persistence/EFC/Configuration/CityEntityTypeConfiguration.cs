using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nutrisense.Nutrisense.Platform.SmartRecommendations.Domain.Model.Aggregates;

namespace Nutrisense.Nutrisense.Platform.SmartRecommendations.Infrastructure.Persistence.EFC.Configuration;

public class CityEntityTypeConfiguration : IEntityTypeConfiguration<City>
{
    public void Configure(EntityTypeBuilder<City> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.Property(c => c.Key)
            .HasColumnName("key")
            .HasMaxLength(50)
            .IsRequired();

        builder.HasIndex(c => c.Key)
            .IsUnique()
            .HasDatabaseName("ix_cities_key");

        builder.Property(c => c.NameEn)
            .HasColumnName("name_en")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(c => c.NameEs)
            .HasColumnName("name_es")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(c => c.Country)
            .HasColumnName("country")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(c => c.Lat)
            .HasColumnName("lat")
            .HasColumnType("decimal(9,6)")
            .IsRequired();

        builder.Property(c => c.Lng)
            .HasColumnName("lng")
            .HasColumnType("decimal(9,6)")
            .IsRequired();

        builder.Property(c => c.Timezone)
            .HasColumnName("timezone")
            .HasMaxLength(50)
            .IsRequired();
    }
}
