using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nutrisense.Nutrisense.Platform.BodyHealthMetrics.Domain.Model.Entities;
using Nutrisense.Nutrisense.Platform.BodyHealthMetrics.Domain.Model.ValueObjects;

namespace Nutrisense.Nutrisense.Platform.BodyHealthMetrics.Infrastructure.Persistence.EFC.Configuration;

/// <summary>EF Core table mapping for the UserGoal entity.</summary>
public class UserGoalEntityTypeConfiguration : IEntityTypeConfiguration<UserGoal>
{
    public void Configure(EntityTypeBuilder<UserGoal> builder)
    {
        builder.ToTable("user_goals");
        builder.HasKey(g => g.Id);

        builder.Property(g => g.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.Property(g => g.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        builder.Property(g => g.Goal)
            .HasConversion(
                goal => goal.Value,
                v => new GoalType(v))
            .HasColumnName("goal")
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(g => g.StartWeightKg)
            .HasColumnName("start_weight_kg")
            .HasColumnType("decimal(5,2)")
            .IsRequired();

        builder.Property(g => g.TargetWeightKg)
            .HasColumnName("target_weight_kg")
            .HasColumnType("decimal(5,2)")
            .IsRequired();

        builder.Property(g => g.WeeklyRateKg)
            .HasConversion(
                wr => wr.Value,
                v => new WeeklyRate(v))
            .HasColumnName("weekly_rate_kg")
            .HasColumnType("decimal(4,2)")
            .IsRequired();

        builder.Property(g => g.DailyCalorieTarget)
            .HasColumnName("daily_calorie_target")
            .IsRequired();

        builder.Property(g => g.ProteinTargetG)
            .HasColumnName("protein_target_g")
            .HasColumnType("decimal(6,1)")
            .IsRequired();

        builder.Property(g => g.CarbsTargetG)
            .HasColumnName("carbs_target_g")
            .HasColumnType("decimal(6,1)")
            .IsRequired();

        builder.Property(g => g.FatTargetG)
            .HasColumnName("fat_target_g")
            .HasColumnType("decimal(6,1)")
            .IsRequired();

        builder.Property(g => g.FiberTargetG)
            .HasColumnName("fiber_target_g")
            .HasColumnType("decimal(6,1)")
            .IsRequired();

        builder.Property(g => g.CaloricAdjustment)
            .HasColumnName("caloric_adjustment")
            .HasColumnType("decimal(8,2)")
            .IsRequired();

        builder.Property(g => g.SetAt)
            .HasColumnName("set_at")
            .IsRequired();

        builder.Property(g => g.Active)
            .HasColumnName("active")
            .IsRequired();
    }
}
