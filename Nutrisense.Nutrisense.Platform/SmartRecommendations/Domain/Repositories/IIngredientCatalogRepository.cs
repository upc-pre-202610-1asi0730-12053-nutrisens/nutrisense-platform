using Nutrisense.Nutrisense.Platform.Shared.Domain.Repositories;
using Nutrisense.Nutrisense.Platform.SmartRecommendations.Domain.Model.Aggregates;

namespace Nutrisense.Nutrisense.Platform.SmartRecommendations.Domain.Repositories;

public interface IIngredientCatalogRepository : IBaseRepository<IngredientCatalogItem>
{
    Task<IEnumerable<IngredientCatalogItem>> FindByCategoryAsync(string category, CancellationToken ct = default);
}
