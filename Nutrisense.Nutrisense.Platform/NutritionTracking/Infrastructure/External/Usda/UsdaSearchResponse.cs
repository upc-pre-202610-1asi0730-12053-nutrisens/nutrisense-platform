namespace Nutrisense.Nutrisense.Platform.NutritionTracking.Infrastructure.External.Usda;

/// <summary>DTOs for the USDA FoodData Central <c>/foods/search</c> response (only the fields we use).</summary>
public sealed record UsdaSearchResponse(List<UsdaFoodItem>? Foods);

public sealed record UsdaFoodItem(
    int? FdcId,
    string? Description,
    string? DataType,
    List<UsdaFoodNutrient>? FoodNutrients);

public sealed record UsdaFoodNutrient(
    string? NutrientName,
    string? UnitName,
    double? Value);
