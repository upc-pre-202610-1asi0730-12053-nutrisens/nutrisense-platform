using Microsoft.EntityFrameworkCore;
using Nutrisense.Nutrisense.Platform.Shared.Domain.Repositories;
using Nutrisense.Nutrisense.Platform.Shared.Infrastructure.Persistence.EFC.Configuration;
using Nutrisense.Nutrisense.Platform.Shared.Infrastructure.Persistence.EFC.Repositories;
using Nutrisense.Nutrisense.Platform.SmartRecommendations.Domain.Model.Aggregates;
using Nutrisense.Nutrisense.Platform.SmartRecommendations.Domain.Repositories;

namespace Nutrisense.Nutrisense.Platform.SmartRecommendations.Infrastructure.Persistence.EFC.Repositories;

public class IngredientCatalogRepository(AppDbContext context)
    : BaseRepository<IngredientCatalogItem>(context), IIngredientCatalogRepository
{
    public async Task<IEnumerable<IngredientCatalogItem>> FindByCategoryAsync(
        string category, CancellationToken ct = default) =>
        await Context.Set<IngredientCatalogItem>()
            .Where(i => i.Category == category)
            .ToListAsync(ct);

    Task<IngredientCatalogItem?> IBaseRepository<IngredientCatalogItem>.FindByIdAsync(int id, CancellationToken ct) =>
        base.FindByIdAsync(id, ct);
}
