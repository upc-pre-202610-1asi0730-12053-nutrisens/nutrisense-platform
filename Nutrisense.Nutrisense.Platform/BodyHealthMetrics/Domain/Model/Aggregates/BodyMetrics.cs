using Nutrisense.Nutrisense.Platform.BodyHealthMetrics.Domain.Model.Commands;
using Nutrisense.Nutrisense.Platform.BodyHealthMetrics.Domain.Model.Entities;
using Nutrisense.Nutrisense.Platform.BodyHealthMetrics.Domain.Model.ValueObjects;
using Nutrisense.Nutrisense.Platform.Shared.Domain.Model.Entities;

namespace Nutrisense.Nutrisense.Platform.BodyHealthMetrics.Domain.Model.Aggregates;

/// <summary>Aggregate root holding a user's physical profile, weight history, body measurements, and computed health metrics.</summary>
public class BodyMetrics : IAuditableEntity
{
    public int Id { get; private set; }
    public int UserId { get; private set; }
    public decimal HeightCm { get; private set; }
    public DateOnly? DateOfBirth { get; private set; }
    public string? BiologicalSex { get; private set; }
    public string? ActivityLevel { get; private set; }

    // BMI backing fields — BmiResult VO reconstructed on read to avoid EF nullable-OwnsOne complexity
    public decimal? BmiValue { get; private set; }
    public string? BmiCategory { get; private set; }

    public decimal? Bmr { get; private set; }
    public decimal? Tdee { get; private set; }

    // MacroTargets backing fields — MacroTargets VO reconstructed on read
    public int? MacroCalories { get; private set; }
    public decimal? MacroProteinG { get; private set; }
    public decimal? MacroCarbsG { get; private set; }
    public decimal? MacroFatG { get; private set; }
    public decimal? MacroFiberG { get; private set; }

    public List<WeightLog> WeightLogs { get; private set; } = [];
    public List<BodyMeasurement> BodyMeasurements { get; private set; } = [];
    public List<UserGoal> UserGoals { get; private set; } = [];

    public DateTimeOffset? CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }

    // Computed VOs — not mapped to DB
    public BmiResult? Bmi =>
        BmiValue.HasValue && BmiCategory != null ? new BmiResult(BmiValue.Value, BmiCategory) : null;

    public MacroTargets? MacroTargets =>
        MacroCalories.HasValue
            ? new MacroTargets(MacroCalories.Value, MacroProteinG!.Value, MacroCarbsG!.Value, MacroFatG!.Value, MacroFiberG!.Value)
            : null;

    protected BodyMetrics() { }

    public BodyMetrics(RegisterBodyMetricsCommand command)
    {
        UserId = command.UserId;
        HeightCm = command.HeightCm;
        DateOfBirth = command.DateOfBirth;
        BiologicalSex = command.BiologicalSex;
        ActivityLevel = command.ActivityLevel;
        WeightLogs.Add(new WeightLog(UserId, new WeightKg(command.WeightKg), DateTimeOffset.UtcNow, null));
    }

    public WeightLog LogWeight(decimal weightKg, string? note)
    {
        var log = new WeightLog(UserId, new WeightKg(weightKg), DateTimeOffset.UtcNow, note);
        WeightLogs.Add(log);
        return log;
    }

    public BodyMeasurement LogMeasurement(decimal waistCm, decimal neckCm)
    {
        var measurement = new BodyMeasurement(
            UserId, new WaistMeasurement(waistCm), new NeckMeasurement(neckCm), DateTimeOffset.UtcNow);
        BodyMeasurements.Add(measurement);
        return measurement;
    }

    public void SetHealthGoal(SetHealthGoalCommand command)
    {
        foreach (var goal in UserGoals.Where(g => g.Active))
            goal.Deactivate();

        var startWeight = GetCurrentWeightKg() ?? command.TargetWeightKg;
        UserGoals.Add(new UserGoal(
            UserId,
            new GoalType(command.Goal),
            startWeight,
            command.TargetWeightKg,
            new WeeklyRate(command.WeeklyRateKg),
            DateTimeOffset.UtcNow));
    }

    public decimal? GetCurrentWeightKg() =>
        WeightLogs.Count == 0 ? null : WeightLogs.MaxBy(w => w.LoggedAt)?.WeightKg.Value;

    public UserGoal? GetActiveGoal() =>
        UserGoals.FirstOrDefault(g => g.Active);

    public void SetBmi(BmiResult bmi)
    {
        BmiValue = bmi.Value;
        BmiCategory = bmi.Category;
    }

    public void SetBmr(decimal bmr) => Bmr = bmr;
    public void SetTdee(decimal tdee) => Tdee = tdee;

    public void SetDailyCaloricGoal(MacroTargets macros, decimal caloricAdjustment)
    {
        MacroCalories = macros.Calories;
        MacroProteinG = macros.ProteinG;
        MacroCarbsG = macros.CarbsG;
        MacroFatG = macros.FatG;
        MacroFiberG = macros.FiberG;
        GetActiveGoal()?.SetCalculatedTargets(
            macros.Calories, macros.ProteinG, macros.CarbsG, macros.FatG, macros.FiberG, caloricAdjustment);
    }
}
