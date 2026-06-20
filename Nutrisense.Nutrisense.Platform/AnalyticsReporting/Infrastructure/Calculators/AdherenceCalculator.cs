using Nutrisense.Nutrisense.Platform.AnalyticsReporting.Domain.Services;
using Nutrisense.Nutrisense.Platform.BodyHealthMetrics.Interfaces.Acl;
using Nutrisense.Nutrisense.Platform.NutritionTracking.Interfaces.Acl;

namespace Nutrisense.Nutrisense.Platform.AnalyticsReporting.Infrastructure.Calculators;

/// <summary>Scores adherence by weighting how close actual calories and protein were to targets.</summary>
public class AdherenceCalculator : IAdherenceCalculator
{
    public decimal Calculate(DailyMacroSummaryItem? actual, UserGoalSummaryItem? goal)
    {
        if (actual is null || goal is null) return 0;
        if (goal.DailyCalorieTarget == 0 || goal.ProteinTargetG == 0) return 0;

        var calorieRatio = actual.TotalCalories / goal.DailyCalorieTarget;
        var proteinRatio = actual.TotalProteinG / goal.ProteinTargetG;
        var score = calorieRatio * 0.5m + proteinRatio * 0.5m;

        return Math.Clamp(score, 0, 1);
    }
}
