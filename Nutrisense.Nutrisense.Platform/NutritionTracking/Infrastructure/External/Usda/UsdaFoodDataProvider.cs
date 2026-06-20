using System.Net.Http.Json;
using Nutrisense.Nutrisense.Platform.NutritionTracking.Domain.Services;

namespace Nutrisense.Nutrisense.Platform.NutritionTracking.Infrastructure.External.Usda;

/// <summary>
/// Resolves foods from USDA FoodData Central (POST /foods/search) and normalizes their
/// nutrients to per-100g macros. USDA Foundation/SR Legacy data is reported per 100g.
/// </summary>
public class UsdaFoodDataProvider(
    HttpClient httpClient,
    IConfiguration configuration,
    ILogger<UsdaFoodDataProvider> logger) : IExternalFoodDataProvider
{
    private const string SourceLabel = "usda";
    private const decimal ServingSize = 100m;
    private const string ServingUnit = "g";

    // Maps USDA nutrient names to our internal macro keys.
    private static readonly Dictionary<string, string> NutrientMap = new()
    {
        ["Energy"] = "calories",
        ["Energy (Atwater General Factors)"] = "calories",
        ["Energy (Atwater Specific Factors)"] = "calories",
        ["Protein"] = "protein",
        ["Carbohydrate, by difference"] = "carbs",
        ["Total lipid (fat)"] = "fat",
        ["Fiber, total dietary"] = "fiber",
        ["Sugars, total including NLEA"] = "sugar",
        ["Sugars, total"] = "sugar",
        ["Total Sugars"] = "sugar"
    };

    public async Task<IReadOnlyList<ExternalFoodData>> SearchAsync(
        string query, int maxResults, string dataType, CancellationToken ct = default)
    {
        var apiKey = configuration["Usda:ApiKey"];
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            logger.LogWarning("USDA API key not configured; returning no results for query '{Query}'.", query);
            return [];
        }

        var baseUrl = configuration["Usda:BaseUrl"]?.TrimEnd('/') ?? "https://api.nal.usda.gov/fdc/v1";
        var url = $"{baseUrl}/foods/search?api_key={apiKey}";
        var body = new { query, dataType = new[] { dataType }, pageSize = maxResults };

        var response = await httpClient.PostAsJsonAsync(url, body, ct);
        response.EnsureSuccessStatusCode();

        var payload = await response.Content.ReadFromJsonAsync<UsdaSearchResponse>(ct);
        if (payload?.Foods is null) return [];

        return payload.Foods
            .Where(f => !string.IsNullOrWhiteSpace(f.Description))
            .Select(ToExternalFoodData)
            .ToList();
    }

    private static ExternalFoodData ToExternalFoodData(UsdaFoodItem item)
    {
        var n = ExtractNutrients(item.FoodNutrients);
        return new ExternalFoodData(
            item.FdcId?.ToString(),
            item.Description!.Trim(),
            n.GetValueOrDefault("calories"),
            n.GetValueOrDefault("protein"),
            n.GetValueOrDefault("carbs"),
            n.GetValueOrDefault("fat"),
            n.GetValueOrDefault("fiber"),
            n.GetValueOrDefault("sugar"),
            ServingSize, ServingUnit, SourceLabel);
    }

    private static Dictionary<string, decimal> ExtractNutrients(List<UsdaFoodNutrient>? nutrients)
    {
        var result = new Dictionary<string, decimal>();
        if (nutrients is null) return result;

        foreach (var nutrient in nutrients)
        {
            if (nutrient.NutrientName is null || !NutrientMap.TryGetValue(nutrient.NutrientName, out var key))
                continue;
            // Energy is reported in both kcal and kJ; only take the kcal value.
            if (key == "calories" && !string.Equals(nutrient.UnitName, "kcal", StringComparison.OrdinalIgnoreCase))
                continue;
            result.TryAdd(key, (decimal)(nutrient.Value ?? 0));
        }
        return result;
    }
}
