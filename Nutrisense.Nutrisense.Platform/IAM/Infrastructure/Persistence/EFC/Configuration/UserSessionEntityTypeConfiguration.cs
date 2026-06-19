using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Nutrisense.Nutrisense.Platform.IAM.Domain.Model.Entities;
using Nutrisense.Nutrisense.Platform.IAM.Domain.Model.ValueObjects;

namespace Nutrisense.Nutrisense.Platform.IAM.Infrastructure.Persistence.EFC.Configuration;

public class UserSessionEntityTypeConfiguration : IEntityTypeConfiguration<UserSession>
{
    public void Configure(EntityTypeBuilder<UserSession> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.UserId)
            .IsRequired()
            .HasConversion(
                id => id.Value,
                value => new UserId(value));

        builder.Property(s => s.DeviceLabel).HasMaxLength(200).IsRequired();
        builder.Property(s => s.IsCurrent).IsRequired();
        builder.Property(s => s.CreatedAt).IsRequired();
        builder.Property(s => s.LastActiveAt).IsRequired();
    }
}
