namespace Nutrisense.Nutrisense.Platform.SmartRecommendations.Interfaces.REST.Resources;

/// <summary>Request to generate a recommendation for a user.</summary>
public record GenerateRecommendationResource(
    /// <summary>Trigger reason for recommendation (e.g., "breakfast", "lunch", "snack", "workout_recovery"). Valid values: breakfast, lunch, dinner, snack, workout_recovery.</summary>
    string Trigger);
