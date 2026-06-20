using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nutrisense.Nutrisense.Platform.SmartRecommendations.Domain.Model.Aggregates;

namespace Nutrisense.Nutrisense.Platform.SmartRecommendations.Infrastructure.Persistence.EFC.Configuration;

public class LocationPreferenceEntityTypeConfiguration : IEntityTypeConfiguration<LocationPreference>
{
    public void Configure(EntityTypeBuilder<LocationPreference> builder)
    {
        builder.HasKey(lp => lp.Id);

        builder.Property(lp => lp.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.Property(lp => lp.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        builder.HasIndex(lp => lp.UserId)
            .IsUnique()
            .HasDatabaseName("ix_location_preferences_user_id");

        builder.Property(lp => lp.HomeCityId)
            .HasColumnName("home_city_id")
            .IsRequired(false);

        builder.Property(lp => lp.CurrentCityId)
            .HasColumnName("current_city_id")
            .IsRequired(false);

        builder.Property(lp => lp.TravelModeActive)
            .HasColumnName("travel_mode_active")
            .IsRequired();

        builder.Property(lp => lp.IsManualOverride)
            .HasColumnName("is_manual_override")
            .IsRequired();

        builder.Property(lp => lp.LocationPermissionGranted)
            .HasColumnName("location_permission_granted")
            .IsRequired();

        builder.Property(lp => lp.CreatedAt).HasColumnName("created_at");
        builder.Property(lp => lp.UpdatedAt).HasColumnName("updated_at");
    }
}
