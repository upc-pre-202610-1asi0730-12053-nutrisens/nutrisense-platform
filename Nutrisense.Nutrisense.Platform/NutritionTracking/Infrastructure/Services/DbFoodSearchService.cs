using Nutrisense.Nutrisense.Platform.NutritionTracking.Domain.Repositories;
using Nutrisense.Nutrisense.Platform.NutritionTracking.Domain.Services;

namespace Nutrisense.Nutrisense.Platform.NutritionTracking.Infrastructure.Services;

/// <summary>
/// Resolves food matches for the meal-scan calorie fallback against the locally imported
/// catalog (USDA-sourced, persisted in the database). No external calls are made: the
/// catalog is populated ahead of time by the import pipeline.
/// </summary>
public class DbFoodSearchService(IFoodRepository foodRepository) : IFoodSearchService
{
    public async Task<IEnumerable<FoodSearchResult>> SearchAsync(string query, string lang, CancellationToken ct)
    {
        // Matching definitivo: IFoodRepository.SearchByNameAsync normaliza la query, filtra por
        // substring sobre el catálogo en BD y rankea (exacto > prefijo > substring, nombres más
        // cortos primero), con tope de resultados y lista vacía cuando no hay query/match.
        // Pendiente futuro (requiere soporte de BD): matching difuso/tolerante a typos (trigram/Levenshtein).
        var matches = await foodRepository.SearchByNameAsync(query, lang, ct);
        return matches.Select(f => new FoodSearchResult(
            f.Id, f.NameEn, f.NameEs,
            f.CaloriesPer100g, f.ProteinPer100g, f.CarbsPer100g, f.FatPer100g,
            f.Source));
    }
}
