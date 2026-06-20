namespace Nutrisense.Nutrisense.Platform.NutritionTracking.Domain.Services;

public record FoodSearchResult(
    int? FoodId,
    string NameEn,
    string NameEs,
    decimal CaloriesPer100g,
    decimal ProteinPer100g,
    decimal CarbsPer100g,
    decimal FatPer100g,
    string Source);

public interface IFoodSearchService
{
    Task<IEnumerable<FoodSearchResult>> SearchAsync(string query, string lang, CancellationToken ct);
}
