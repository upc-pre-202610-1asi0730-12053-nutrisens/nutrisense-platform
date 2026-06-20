namespace Nutrisense.Nutrisense.Platform.NutritionTracking.Interfaces.REST.Resources;

public record ImportFoodsResource(
    string Query,
    int MaxResults = 50,
    string DataType = "Foundation")
{
    /// <summary>Search query to find foods in USDA FoodData Central (e.g., "banana", "apple").</summary>
    public string Query { get; } = Query;

    /// <summary>Maximum number of results to import (default: 50, must be between 1-500).</summary>
    public int MaxResults { get; } = MaxResults;

    /// <summary>Data type from USDA FoodData Central (default: Foundation). Valid values: Foundation, SR Legacy.</summary>
    public string DataType { get; } = DataType;
}
