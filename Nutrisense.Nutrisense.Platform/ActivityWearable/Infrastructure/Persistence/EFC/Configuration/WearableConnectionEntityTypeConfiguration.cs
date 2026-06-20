using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nutrisense.Nutrisense.Platform.ActivityWearable.Domain.Model.Aggregates;

namespace Nutrisense.Nutrisense.Platform.ActivityWearable.Infrastructure.Persistence.EFC.Configuration;

/// <summary>EF Core mapping for the <see cref="WearableConnection"/> aggregate: keys, column names and lengths.</summary>
public class WearableConnectionEntityTypeConfiguration : IEntityTypeConfiguration<WearableConnection>
{
    /// <summary>Configures the table mapping, column conventions and the (UserId, Provider) index for <see cref="WearableConnection"/>.</summary>
    /// <param name="builder">The entity type builder supplied by EF Core.</param>
    public void Configure(EntityTypeBuilder<WearableConnection> builder)
    {
        builder.HasKey(w => w.Id);

        builder.Property(w => w.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.Property(w => w.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        builder.Property(w => w.Provider)
            .HasColumnName("provider")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(w => w.Status)
            .HasColumnName("status")
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(w => w.LastSyncedAt)
            .HasColumnName("last_synced_at")
            .IsRequired(false);

        builder.Property(w => w.AuthorizedAt)
            .HasColumnName("authorized_at")
            .IsRequired();

        builder.Property(w => w.CreatedAt).HasColumnName("created_at");
        builder.Property(w => w.UpdatedAt).HasColumnName("updated_at");

        // Index on (UserId, Provider) — uniqueness when Status = "connected" enforced in app layer.
        builder.HasIndex(w => new { w.UserId, w.Provider })
            .HasDatabaseName("ix_wearable_connections_user_provider");
    }
}
