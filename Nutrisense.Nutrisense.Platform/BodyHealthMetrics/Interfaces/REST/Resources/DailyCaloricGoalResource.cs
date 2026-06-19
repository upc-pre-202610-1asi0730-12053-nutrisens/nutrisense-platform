namespace Nutrisense.Nutrisense.Platform.BodyHealthMetrics.Interfaces.REST.Resources;

/// <summary>API response carrying a user's computed daily caloric and macro targets.</summary>
public record DailyCaloricGoalResource(
    /// <summary>Daily caloric target in kilocalories.</summary>
    int DailyCalories,
    /// <summary>Daily protein target in grams.</summary>
    decimal ProteinG,
    /// <summary>Daily carbohydrate target in grams.</summary>
    decimal CarbsG,
    /// <summary>Daily fat target in grams.</summary>
    decimal FatG,
    /// <summary>Daily fiber target in grams.</summary>
    decimal FiberG);
