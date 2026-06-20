using Nutrisense.Nutrisense.Platform.NutritionTracking.Domain.Model.Aggregates;
using Nutrisense.Nutrisense.Platform.NutritionTracking.Domain.Model.Queries;

namespace Nutrisense.Nutrisense.Platform.NutritionTracking.Application.QueryServices;

public interface IFoodQueryService
{
    Task<IEnumerable<Food>> Handle(SearchFoodQuery query, CancellationToken ct = default);
    Task<Food?> Handle(GetFoodByIdQuery query, CancellationToken ct = default);
}
