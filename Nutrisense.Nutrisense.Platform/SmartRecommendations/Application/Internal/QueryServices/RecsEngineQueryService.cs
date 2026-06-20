using Nutrisense.Nutrisense.Platform.SmartRecommendations.Application.QueryServices;
using Nutrisense.Nutrisense.Platform.SmartRecommendations.Domain.Model.Aggregates;
using Nutrisense.Nutrisense.Platform.SmartRecommendations.Domain.Model.Queries;
using Nutrisense.Nutrisense.Platform.SmartRecommendations.Domain.Repositories;
using Nutrisense.Nutrisense.Platform.SmartRecommendations.Domain.Services;

namespace Nutrisense.Nutrisense.Platform.SmartRecommendations.Application.Internal.QueryServices;

public class RecsEngineQueryService(
    IRecommendationCardRepository recommendationCardRepository,
    IPantryRepository pantryRepository,
    ILocationPreferenceRepository locationPreferenceRepository,
    ICityRepository cityRepository,
    IRecipeRepository recipeRepository,
    IIngredientCatalogRepository ingredientCatalogRepository,
    IWeatherService weatherService,
    IGeocodingService geocodingService) : IRecsEngineQueryService
{
    public async Task<IEnumerable<RecommendationCard>> Handle(
        GetActiveRecommendationsByUserIdQuery query, CancellationToken ct = default) =>
        await recommendationCardRepository.FindActiveListByUserIdAsync(query.UserId, ct);

    public async Task<IEnumerable<RecommendationCard>> Handle(
        GetAllRecommendationsByUserIdQuery query, CancellationToken ct = default) =>
        await recommendationCardRepository.FindByUserIdAsync(query.UserId, ct);

    public async Task<Pantry?> Handle(GetPantryByUserIdQuery query, CancellationToken ct = default) =>
        await pantryRepository.FindByUserIdAsync(query.UserId, ct);

    public async Task<LocationPreference?> Handle(
        GetLocationPreferenceByUserIdQuery query, CancellationToken ct = default) =>
        await locationPreferenceRepository.FindByUserIdAsync(query.UserId, ct);

    public async Task<IEnumerable<City>> Handle(GetAllCitiesQuery query, CancellationToken ct = default) =>
        await cityRepository.ListAsync(ct);

    public async Task<City?> Handle(GetCityByIdQuery query, CancellationToken ct = default) =>
        await cityRepository.FindByIdAsync(query.Id, ct);

    public async Task<WeatherSnapshot?> Handle(GetCurrentWeatherByCityIdQuery query, CancellationToken ct = default)
    {
        var city = await cityRepository.FindByIdAsync(query.CityId, ct);
        if (city is null) return null;
        return await weatherService.GetCurrentAsync(query.CityId, ct);
    }

    public async Task<IEnumerable<CitySearchResult>> Handle(SearchCitiesQuery query, CancellationToken ct = default)
    {
        var q = query.Query?.Trim() ?? "";
        if (q.Length < 2) return [];

        var all = (await cityRepository.ListAsync(ct)).ToList();
        var results = new List<CitySearchResult>();
        var seen = new HashSet<string>();

        // Local catalog matches first (already imported, carry a real Id).
        foreach (var c in all.Where(c => Contains(c.NameEn, q) || Contains(c.NameEs, q) || Contains(c.Key, q)))
        {
            if (seen.Add(c.Key))
                results.Add(new CitySearchResult(c.Id, c.Key, c.NameEn, c.NameEs, c.Country, null, c.Lat, c.Lng));
        }

        // Geocoding candidates; reconcile against the catalog by natural key to avoid duplicates.
        var byKey = all.ToDictionary(c => c.Key);
        foreach (var cand in await geocodingService.SearchAsync(q, query.Limit, ct))
        {
            var key = City.BuildKey(cand.Name, cand.Country, cand.Lat, cand.Lng);
            if (!seen.Add(key)) continue;
            results.Add(byKey.TryGetValue(key, out var existing)
                ? new CitySearchResult(existing.Id, existing.Key, existing.NameEn, existing.NameEs, existing.Country, cand.State, existing.Lat, existing.Lng)
                : new CitySearchResult(null, null, cand.NameEn ?? cand.Name, cand.NameEs ?? cand.Name, cand.Country, cand.State, cand.Lat, cand.Lng));
        }

        return results;
    }

    private static bool Contains(string? source, string term) =>
        source?.Contains(term, StringComparison.OrdinalIgnoreCase) == true;

    public async Task<IEnumerable<Recipe>> Handle(GetRecipesByGoalQuery query, CancellationToken ct = default)
    {
        var recipes = await recipeRepository.FindByGoalTypeAsync(query.GoalType, ct);
        if (query.MaxPrepMinutes.HasValue)
            recipes = recipes.Where(r => r.PrepTimeMinutes <= query.MaxPrepMinutes.Value);
        return recipes;
    }

    public async Task<Recipe?> Handle(GetRecipeByIdQuery query, CancellationToken ct = default) =>
        await recipeRepository.FindByIdAsync(query.Id, ct);

    public async Task<IEnumerable<IngredientCatalogItem>> Handle(
        GetIngredientCatalogQuery query, CancellationToken ct = default) =>
        await ingredientCatalogRepository.ListAsync(ct);
}
