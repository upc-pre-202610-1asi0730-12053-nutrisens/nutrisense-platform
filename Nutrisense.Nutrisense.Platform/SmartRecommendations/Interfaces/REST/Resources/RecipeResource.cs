namespace Nutrisense.Nutrisense.Platform.SmartRecommendations.Interfaces.REST.Resources;

/// <summary>A recipe with ingredients and nutritional information for a health goal.</summary>
public record RecipeResource(
    /// <summary>Unique identifier for the recipe.</summary>
    int Id,
    /// <summary>Unique key/slug for the recipe (e.g., "grilled-chicken-salad").</summary>
    string Key,
    /// <summary>Recipe name in English.</summary>
    string NameEn,
    /// <summary>Recipe name in Spanish.</summary>
    string NameEs,
    /// <summary>Health goal this recipe targets (e.g., "weight-loss", "muscle-gain", "maintenance"). Valid values: weight-loss, muscle-gain, maintenance, endurance.</summary>
    string GoalType,
    /// <summary>Estimated preparation time in minutes.</summary>
    int PrepTimeMinutes,
    /// <summary>Number of servings this recipe yields.</summary>
    int Servings,
    /// <summary>Total calories for all servings in kcal.</summary>
    decimal TotalCalories,
    /// <summary>Total protein content in grams for all servings.</summary>
    decimal TotalProteinG,
    /// <summary>Total carbohydrates in grams for all servings.</summary>
    decimal TotalCarbsG,
    /// <summary>Total fat content in grams for all servings.</summary>
    decimal TotalFatG,
    /// <summary>Total dietary fiber in grams for all servings.</summary>
    decimal TotalFiberG,
    /// <summary>List of user dietary restrictions this recipe conflicts with.</summary>
    List<string> RestrictionsConflict,
    /// <summary>Collection of ingredients needed for the recipe.</summary>
    IEnumerable<RecipeIngredientResource> Ingredients);
