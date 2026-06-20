using Nutrisense.Nutrisense.Platform.BodyHealthMetrics.Domain.Model.ValueObjects;
using Nutrisense.Nutrisense.Platform.BodyHealthMetrics.Domain.Services;

namespace Nutrisense.Nutrisense.Platform.BodyHealthMetrics.Infrastructure.Calculators;

/// <summary>Computes BMI, BMR (Mifflin-St Jeor formula), TDEE, and daily macro targets from raw biometric inputs.</summary>
public class MifflinStJeorBodyMetricsCalculator : IBodyMetricsCalculator
{
    public BmiResult CalculateBmi(decimal weightKg, decimal heightCm)
    {
        var heightM = heightCm / 100m;
        var bmi = Math.Round(weightKg / (heightM * heightM), 2);
        var category = bmi switch
        {
            < 18.5m => "underweight",
            < 25.0m => "normal",
            < 30.0m => "overweight",
            _ => "obese"
        };
        return new BmiResult(bmi, category);
    }

    public decimal CalculateBmr(decimal weightKg, decimal heightCm, DateOnly dateOfBirth, string biologicalSex)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var age = today.Year - dateOfBirth.Year;
        if (today < dateOfBirth.AddYears(age)) age--;

        // Mifflin-St Jeor: 10·weight + 6.25·height − 5·age + s
        var s = biologicalSex.ToLowerInvariant() switch
        {
            "male" => 5m,
            "female" => -161m,
            _ => -78m   // prefer-not-to-say
        };

        return Math.Round(10m * weightKg + 6.25m * heightCm - 5m * age + s, 2);
    }

    public decimal CalculateTdee(decimal bmr, string activityLevel)
    {
        var factor = activityLevel.ToLowerInvariant() switch
        {
            "sedentary" => 1.2m,
            "lightly" => 1.375m,
            "moderately" => 1.55m,
            "very" => 1.725m,
            _ => 1.2m
        };
        return Math.Round(bmr * factor, 2);
    }

    public decimal CalculateCaloricAdjustment(string goal, decimal weeklyRateKg) =>
        goal.ToLowerInvariant() switch
        {
            "weight-loss" => Math.Round(-500m * Math.Abs(weeklyRateKg) / 0.5m, 2),
            "muscle-gain" => Math.Round(300m * weeklyRateKg / 0.5m, 2),
            _ => 0m
        };

    public MacroTargets CalculateDailyCaloricGoal(decimal tdee, string goal, decimal weeklyRateKg)
    {
        var adjustment = CalculateCaloricAdjustment(goal, weeklyRateKg);
        var calories = (int)Math.Round(tdee + adjustment);

        // 30% protein (4 kcal/g), 40% carbs (4 kcal/g), 30% fat (9 kcal/g), 14 g fiber per 1000 kcal
        var proteinG = Math.Round(calories * 0.30m / 4m, 1);
        var carbsG = Math.Round(calories * 0.40m / 4m, 1);
        var fatG = Math.Round(calories * 0.30m / 9m, 1);
        var fiberG = Math.Round(calories / 1000m * 14m, 1);

        return new MacroTargets(calories, proteinG, carbsG, fatG, fiberG);
    }
}
