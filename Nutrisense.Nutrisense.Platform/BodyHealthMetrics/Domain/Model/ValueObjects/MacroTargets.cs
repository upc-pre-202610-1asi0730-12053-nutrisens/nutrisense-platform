namespace Nutrisense.Nutrisense.Platform.BodyHealthMetrics.Domain.Model.ValueObjects;

/// <summary>Daily macronutrient targets (calories, protein, carbs, fat, fiber) validated for internal caloric consistency.</summary>
public sealed record MacroTargets
{
    public int Calories { get; }
    public decimal ProteinG { get; }
    public decimal CarbsG { get; }
    public decimal FatG { get; }
    public decimal FiberG { get; }

    public MacroTargets(int calories, decimal proteinG, decimal carbsG, decimal fatG, decimal fiberG)
    {
        if (calories < 0) throw new ArgumentException("Calories must be non-negative.", nameof(calories));
        if (proteinG < 0) throw new ArgumentException("Protein must be non-negative.", nameof(proteinG));
        if (carbsG < 0) throw new ArgumentException("Carbs must be non-negative.", nameof(carbsG));
        if (fatG < 0) throw new ArgumentException("Fat must be non-negative.", nameof(fatG));
        if (fiberG < 0) throw new ArgumentException("Fiber must be non-negative.", nameof(fiberG));

        if (calories > 0)
        {
            var macroCalories = proteinG * 4m + carbsG * 4m + fatG * 9m;
            var tolerance = Math.Max(calories * 0.05m, 5m);
            if (Math.Abs(macroCalories - calories) > tolerance)
                throw new ArgumentException("Macro calories are inconsistent with total calories (±5% tolerance).", nameof(calories));
        }

        Calories = calories;
        ProteinG = proteinG;
        CarbsG = carbsG;
        FatG = fatG;
        FiberG = fiberG;
    }
}
