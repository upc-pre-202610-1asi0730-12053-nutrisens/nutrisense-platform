using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nutrisense.Nutrisense.Platform.IAM.Domain.Model.Entities;
using Nutrisense.Nutrisense.Platform.IAM.Domain.Model.ValueObjects;

namespace Nutrisense.Nutrisense.Platform.IAM.Infrastructure.Persistence.EFC.Configuration;

public class DietaryRestrictionEntityTypeConfiguration : IEntityTypeConfiguration<DietaryRestriction>
{
    public void Configure(EntityTypeBuilder<DietaryRestriction> builder)
    {
        builder.HasKey(dr => dr.Id);

        builder.Property(dr => dr.UserId)
            .IsRequired()
            .HasConversion(
                id => id.Value,
                value => new UserId(value));

        builder.Property(dr => dr.Restriction).HasMaxLength(100).IsRequired();
    }
}
