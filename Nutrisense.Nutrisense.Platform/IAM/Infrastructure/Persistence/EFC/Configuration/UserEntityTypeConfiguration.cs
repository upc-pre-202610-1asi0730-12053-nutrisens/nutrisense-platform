using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

using Nutrisense.Nutrisense.Platform.IAM.Domain.Model.Aggregates;
using Nutrisense.Nutrisense.Platform.IAM.Domain.Model.ValueObjects;

namespace Nutrisense.Nutrisense.Platform.IAM.Infrastructure.Persistence.EFC.Configuration;

public class UserEntityTypeConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);

        builder.Property(u => u.Id)
            .HasColumnName("id")
            .HasConversion(
                id => id.Value,
                value => UserId.FromRaw(value))
            .ValueGeneratedOnAdd();

        builder.Property(u => u.Email)
            .HasConversion(
                email => email.Value,
                value => new Email(value))
            .HasColumnName("email")
            .HasMaxLength(255)
            .IsRequired();

        builder.HasIndex(u => u.Email)
            .IsUnique()
            .HasDatabaseName("ix_users_email");

        builder.Property(u => u.PasswordHash)
            .HasColumnName("password_hash")
            .IsRequired();

        // PersonName as owned type — column names set explicitly so the IsOwned() guard
        // in UseSnakeCaseNamingConvention skips key renaming for this owned type
        builder.OwnsOne(u => u.Name, name =>
        {
            name.WithOwner().HasForeignKey("UserId");
            name.Property<int>("UserId").HasColumnName("id");
            name.Property(n => n.FirstName)
                .HasColumnName("first_name")
                .HasMaxLength(80)
                .IsRequired();
            name.Property(n => n.LastName)
                .HasColumnName("last_name")
                .HasMaxLength(80)
                .IsRequired();
        });

        builder.Property(u => u.DateOfBirth)
            .HasConversion(new ValueConverter<DateOfBirth?, string?>(
                dob => dob == null ? null : dob.Value.ToString("yyyy-MM-dd"),
                s => s == null ? null : new DateOfBirth(DateOnly.ParseExact(s, "yyyy-MM-dd", null))))
            .HasColumnName("date_of_birth")
            .IsRequired(false);

        builder.Property(u => u.BiologicalSex)
            .HasConversion(new ValueConverter<BiologicalSex?, string?>(
                bs => bs == null ? null : bs.Value,
                s => s == null ? null : new BiologicalSex(s)))
            .HasColumnName("biological_sex")
            .HasMaxLength(25)
            .IsRequired(false);

        builder.Property(u => u.Height)
            .HasConversion(new ValueConverter<Height?, decimal?>(
                h => h == null ? (decimal?)null : h.Centimeters,
                v => v == null ? null : new Height(v.Value)))
            .HasColumnName("height_cm")
            .IsRequired(false);

        builder.Property(u => u.ActivityLevel)
            .HasConversion(new ValueConverter<ActivityLevel?, string?>(
                al => al == null ? null : al.Value,
                s => s == null ? null : new ActivityLevel(s)))
            .HasColumnName("activity_level")
            .HasMaxLength(25)
            .IsRequired(false);

        builder.Property(u => u.PreferredUnits)
            .HasConversion(
                pu => pu.Value,
                value => new PreferredUnits(value))
            .HasColumnName("preferred_units")
            .HasMaxLength(10)
            .IsRequired();

        builder.Property(u => u.PreferredLanguage)
            .HasConversion(
                pl => pl.Value,
                value => new PreferredLanguage(value))
            .HasColumnName("preferred_language")
            .HasMaxLength(5)
            .IsRequired();

        builder.Property(u => u.GoalIntent)
            .HasConversion(new ValueConverter<GoalIntent?, string?>(
                g => g == null ? null : g.Value,
                s => s == null ? null : new GoalIntent(s)))
            .HasColumnName("goal_intent")
            .HasMaxLength(20)
            .IsRequired(false);

        builder.Property(u => u.MedicalConditions)
            .HasConversion(
                v => System.Text.Json.JsonSerializer.Serialize(v ?? new List<string>(), (System.Text.Json.JsonSerializerOptions?)null),
                v => string.IsNullOrEmpty(v)
                    ? new List<string>()
                    : System.Text.Json.JsonSerializer.Deserialize<List<string>>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? new List<string>(),
                new ValueComparer<List<string>>(
                    (a, b) => a != null && b != null && a.SequenceEqual(b),
                    c => c.Aggregate(0, (h, e) => HashCode.Combine(h, e.GetHashCode())),
                    c => c.ToList()))
            .HasColumnName("medical_conditions")
            .HasColumnType("json")
            .IsRequired();

        builder.Property(u => u.CreatedAt).HasColumnName("created_at");
        builder.Property(u => u.UpdatedAt).HasColumnName("updated_at");

        builder.HasMany(u => u.Sessions)
            .WithOne()
            .HasForeignKey(s => s.UserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(u => u.DietaryRestrictions)
            .WithOne()
            .HasForeignKey(dr => dr.UserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}
