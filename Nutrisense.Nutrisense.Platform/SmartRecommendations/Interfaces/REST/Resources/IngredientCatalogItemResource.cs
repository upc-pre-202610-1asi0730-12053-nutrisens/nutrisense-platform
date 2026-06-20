namespace Nutrisense.Nutrisense.Platform.SmartRecommendations.Interfaces.REST.Resources;

/// <summary>An ingredient in the global catalog with nutritional and measurement details.</summary>
public record IngredientCatalogItemResource(
    /// <summary>Unique identifier for the ingredient.</summary>
    int Id,
    /// <summary>Unique key/slug for the ingredient (e.g., "chicken-breast", "olive-oil").</summary>
    string Key,
    /// <summary>Ingredient name in English.</summary>
    string NameEn,
    /// <summary>Ingredient name in Spanish.</summary>
    string NameEs,
    /// <summary>Reference to nutrition database food ID, or null if not available.</summary>
    int? FoodId,
    /// <summary>Category of the ingredient (e.g., "protein", "vegetable", "grain", "oil"). Valid categories: protein, vegetable, grain, fruit, dairy, oil, spice.</summary>
    string Category,
    /// <summary>Default unit of measurement (e.g., "g", "ml", "cup", "tbsp").</summary>
    string DefaultUnit,
    /// <summary>Grams per unit for unit conversion, or null if not applicable (e.g., for individual items).</summary>
    decimal? GramsPerUnit);
