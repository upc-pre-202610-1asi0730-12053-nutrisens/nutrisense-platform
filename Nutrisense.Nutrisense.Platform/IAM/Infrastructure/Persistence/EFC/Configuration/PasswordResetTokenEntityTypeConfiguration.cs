using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nutrisense.Nutrisense.Platform.IAM.Domain.Model.Aggregates;

namespace Nutrisense.Nutrisense.Platform.IAM.Infrastructure.Persistence.EFC.Configuration;

public class PasswordResetTokenEntityTypeConfiguration : IEntityTypeConfiguration<PasswordResetToken>
{
    public void Configure(EntityTypeBuilder<PasswordResetToken> builder)
    {
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Id).ValueGeneratedOnAdd();

        builder.Property(t => t.Token)
            .HasMaxLength(64)
            .IsRequired();

        builder.HasIndex(t => t.Token)
            .IsUnique()
            .HasDatabaseName("ix_password_reset_tokens_token");

        builder.Property(t => t.UserId).IsRequired();
        builder.HasIndex(t => t.UserId)
            .HasDatabaseName("ix_password_reset_tokens_user_id");

        builder.Property(t => t.ExpiresAt).IsRequired();
        builder.Property(t => t.Used).IsRequired();
    }
}
