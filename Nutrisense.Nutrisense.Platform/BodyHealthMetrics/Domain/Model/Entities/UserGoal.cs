using Nutrisense.Nutrisense.Platform.BodyHealthMetrics.Domain.Model.ValueObjects;

namespace Nutrisense.Nutrisense.Platform.BodyHealthMetrics.Domain.Model.Entities;

/// <summary>Active or historical health goal defining a user's weight target, weekly pace, and computed daily macro targets.</summary>
public class UserGoal
{
    public int Id { get; private set; }
    public int UserId { get; private set; }
    public GoalType Goal { get; private set; } = null!;
    public decimal StartWeightKg { get; private set; }
    public decimal TargetWeightKg { get; private set; }
    public WeeklyRate WeeklyRateKg { get; private set; } = null!;
    public int DailyCalorieTarget { get; private set; }
    public decimal ProteinTargetG { get; private set; }
    public decimal CarbsTargetG { get; private set; }
    public decimal FatTargetG { get; private set; }
    public decimal FiberTargetG { get; private set; }
    public decimal CaloricAdjustment { get; private set; }
    public DateTimeOffset SetAt { get; private set; }
    public bool Active { get; private set; }

    protected UserGoal() { }

    public UserGoal(
        int userId, GoalType goal, decimal startWeightKg, decimal targetWeightKg,
        WeeklyRate weeklyRateKg, DateTimeOffset setAt)
    {
        UserId = userId;
        Goal = goal;
        StartWeightKg = startWeightKg;
        TargetWeightKg = targetWeightKg;
        WeeklyRateKg = weeklyRateKg;
        SetAt = setAt;
        Active = true;
    }

    public void Deactivate() => Active = false;

    public void SetCalculatedTargets(
        int dailyCalorieTarget, decimal proteinG, decimal carbsG, decimal fatG, decimal fiberG, decimal caloricAdjustment)
    {
        DailyCalorieTarget = dailyCalorieTarget;
        ProteinTargetG = proteinG;
        CarbsTargetG = carbsG;
        FatTargetG = fatG;
        FiberTargetG = fiberG;
        CaloricAdjustment = caloricAdjustment;
    }
}
