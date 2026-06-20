using Nutrisense.Nutrisense.Platform.BodyHealthMetrics.Domain.Model.ValueObjects;

namespace Nutrisense.Nutrisense.Platform.BodyHealthMetrics.Domain.Services;

/// <summary>Contract for computing BMI, BMR, TDEE, and daily macro targets from raw biometric data.</summary>
public interface IBodyMetricsCalculator
{
    BmiResult CalculateBmi(decimal weightKg, decimal heightCm);
    decimal CalculateBmr(decimal weightKg, decimal heightCm, DateOnly dateOfBirth, string biologicalSex);
    decimal CalculateTdee(decimal bmr, string activityLevel);
    decimal CalculateCaloricAdjustment(string goal, decimal weeklyRateKg);
    MacroTargets CalculateDailyCaloricGoal(decimal tdee, string goal, decimal weeklyRateKg);
}
