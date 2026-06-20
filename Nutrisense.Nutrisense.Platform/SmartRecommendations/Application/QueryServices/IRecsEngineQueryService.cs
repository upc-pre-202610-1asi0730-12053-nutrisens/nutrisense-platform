using Nutrisense.Nutrisense.Platform.SmartRecommendations.Application.Internal;
using Nutrisense.Nutrisense.Platform.SmartRecommendations.Domain.Model.Aggregates;
using Nutrisense.Nutrisense.Platform.SmartRecommendations.Domain.Model.Queries;
using Nutrisense.Nutrisense.Platform.SmartRecommendations.Domain.Services;

namespace Nutrisense.Nutrisense.Platform.SmartRecommendations.Application.QueryServices;

public interface IRecsEngineQueryService
{
    Task<IEnumerable<RecommendationCard>> Handle(GetActiveRecommendationsByUserIdQuery query, CancellationToken ct = default);
    Task<IEnumerable<RecommendationCard>> Handle(GetAllRecommendationsByUserIdQuery query, CancellationToken ct = default);
    Task<Pantry?> Handle(GetPantryByUserIdQuery query, CancellationToken ct = default);
    Task<LocationPreference?> Handle(GetLocationPreferenceByUserIdQuery query, CancellationToken ct = default);
    Task<IEnumerable<City>> Handle(GetAllCitiesQuery query, CancellationToken ct = default);
    Task<City?> Handle(GetCityByIdQuery query, CancellationToken ct = default);
    Task<WeatherSnapshot?> Handle(GetCurrentWeatherByCityIdQuery query, CancellationToken ct = default);
    Task<IEnumerable<CitySearchResult>> Handle(SearchCitiesQuery query, CancellationToken ct = default);
    Task<IEnumerable<Recipe>> Handle(GetRecipesByGoalQuery query, CancellationToken ct = default);
    Task<Recipe?> Handle(GetRecipeByIdQuery query, CancellationToken ct = default);
    Task<IEnumerable<IngredientCatalogItem>> Handle(GetIngredientCatalogQuery query, CancellationToken ct = default);
}
