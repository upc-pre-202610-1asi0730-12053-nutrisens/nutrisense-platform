namespace Nutrisense.Nutrisense.Platform.SmartRecommendations.Interfaces.REST.Resources;

/// <summary>An ingredient in a recipe with quantity and unit.</summary>
public record RecipeIngredientResource(
    /// <summary>Unique identifier for the recipe ingredient.</summary>
    int Id,
    /// <summary>Reference to the ingredient in the catalog.</summary>
    int IngredientCatalogItemId,
    /// <summary>Quantity of the ingredient needed for the recipe.</summary>
    decimal Quantity,
    /// <summary>Unit of measurement for the ingredient (e.g., "g", "ml", "cup", "tbsp").</summary>
    string Unit);
