using Nutrisense.Nutrisense.Platform.BodyHealthMetrics.Domain.Model.Aggregates;
using Nutrisense.Nutrisense.Platform.BodyHealthMetrics.Interfaces.REST.Resources;

namespace Nutrisense.Nutrisense.Platform.BodyHealthMetrics.Interfaces.REST.Transform;

/// <summary>Maps the BodyMetrics aggregate to its REST resource representation.</summary>
public static class BodyMetricsResourceAssembler
{
    public static BodyMetricsResource ToResource(BodyMetrics b)
    {
        var goal = b.GetActiveGoal();
        return new(
            b.UserId,
            b.HeightCm,
            b.DateOfBirth?.ToString("yyyy-MM-dd"),
            b.BiologicalSex,
            b.ActivityLevel,
            b.GetCurrentWeightKg(),
            b.BmiValue,
            b.BmiCategory,
            b.Bmr,
            b.Tdee,
            goal?.Goal.Value,
            goal?.StartWeightKg,
            goal?.TargetWeightKg,
            goal?.WeeklyRateKg.Value,
            goal?.SetAt,
            goal?.CaloricAdjustment,
            b.MacroCalories,
            b.MacroProteinG,
            b.MacroCarbsG,
            b.MacroFatG,
            b.MacroFiberG);
    }
}
