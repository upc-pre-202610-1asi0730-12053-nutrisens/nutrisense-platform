using Nutrisense.Nutrisense.Platform.NutritionTracking.Application.QueryServices;
using Nutrisense.Nutrisense.Platform.NutritionTracking.Domain.Model.Aggregates;
using Nutrisense.Nutrisense.Platform.NutritionTracking.Domain.Model.Queries;
using Nutrisense.Nutrisense.Platform.NutritionTracking.Domain.Repositories;

namespace Nutrisense.Nutrisense.Platform.NutritionTracking.Application.Internal.QueryServices;

public class FoodQueryService(IFoodRepository foodRepository) : IFoodQueryService
{
    public async Task<IEnumerable<Food>> Handle(SearchFoodQuery query, CancellationToken ct = default) =>
        await foodRepository.SearchByNameAsync(query.Query, query.PreferredLanguage, ct);

    public async Task<Food?> Handle(GetFoodByIdQuery query, CancellationToken ct = default) =>
        await foodRepository.FindByIdAsync(query.Id, ct);
}
