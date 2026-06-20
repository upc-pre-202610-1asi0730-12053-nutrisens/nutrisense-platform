namespace Nutrisense.Nutrisense.Platform.SmartRecommendations.Interfaces.REST.Resources;

/// <summary>Request to generate AI recipes for specified health goals.</summary>
public record ImportRecipesResource(
    /// <summary>List of goal types to generate recipes for (e.g., "weight-loss", "muscle-gain"). Valid values: weight-loss, muscle-gain, maintenance, endurance. Null or empty to generate for all goals.</summary>
    IReadOnlyList<string>? GoalTypes = null,
    /// <summary>Maximum number of recipes to generate per goal. Default: 10.</summary>
    int MaxPerGoal = 10);
