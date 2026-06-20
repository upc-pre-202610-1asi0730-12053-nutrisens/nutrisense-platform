namespace Nutrisense.Nutrisense.Platform.SmartRecommendations.Interfaces.REST.Resources;

/// <summary>A recommendation card representing a suggested food or recipe for the user.</summary>
public record RecommendationCardResource(
    /// <summary>Unique identifier for the recommendation card.</summary>
    int Id,
    /// <summary>User ID this recommendation belongs to.</summary>
    int UserId,
    /// <summary>Food ID reference, or null if recommendation is recipe-based.</summary>
    int? FoodId,
    /// <summary>Recommended food name in English.</summary>
    string FoodNameEn,
    /// <summary>Recommended food name in Spanish.</summary>
    string FoodNameEs,
    /// <summary>Estimated total calories in kcal.</summary>
    decimal EstimatedCalories,
    /// <summary>Estimated protein content in grams.</summary>
    decimal EstimatedProteinG,
    /// <summary>Estimated carbohydrates in grams.</summary>
    decimal EstimatedCarbsG,
    /// <summary>Estimated fat content in grams.</summary>
    decimal EstimatedFatG,
    /// <summary>Badge label (e.g., "High Protein", "Low Carb"). Possible values: high_protein, low_carb, balanced.</summary>
    string Badge,
    /// <summary>Context/reason for recommendation in English.</summary>
    string ContextLabelEn,
    /// <summary>Context/reason for recommendation in Spanish.</summary>
    string ContextLabelEs,
    /// <summary>List of user dietary restrictions this recommendation conflicts with.</summary>
    List<string> RestrictionsConflict,
    /// <summary>Whether this recommendation is currently active or archived.</summary>
    bool IsActive,
    /// <summary>Recommendation generation timestamp in UTC (ISO 8601 format: yyyy-MM-ddTHH:mm:ss.fffZ).</summary>
    DateTimeOffset CreatedAt);
