using Microsoft.EntityFrameworkCore;
using Nutrisense.Nutrisense.Platform.NutritionTracking.Domain.Model.Aggregates;
using Nutrisense.Nutrisense.Platform.NutritionTracking.Domain.Repositories;
using Nutrisense.Nutrisense.Platform.Shared.Domain.Repositories;
using Nutrisense.Nutrisense.Platform.Shared.Infrastructure.Persistence.EFC.Configuration;
using Nutrisense.Nutrisense.Platform.Shared.Infrastructure.Persistence.EFC.Repositories;

namespace Nutrisense.Nutrisense.Platform.NutritionTracking.Infrastructure.Persistence.EFC.Repositories;

public class FoodRepository(AppDbContext context) : BaseRepository<Food>(context), IFoodRepository
{
    public async Task<Food?> FindByKeyAsync(string key, CancellationToken ct = default) =>
        await Context.Set<Food>().FirstOrDefaultAsync(f => f.Key == key, ct);

    public async Task<IEnumerable<Food>> SearchByNameAsync(string query, string lang, CancellationToken ct = default)
    {
        var lower = query.ToLower();
        return await Context.Set<Food>()
            .Where(f => f.NameEn.ToLower().Contains(lower) || f.NameEs.ToLower().Contains(lower))
            .ToListAsync(ct);
    }

    public async Task<Food?> FindByExternalIdAsync(string externalId, CancellationToken ct = default) =>
        await Context.Set<Food>().FirstOrDefaultAsync(f => f.ExternalId == externalId, ct);

    // Route interface call through base
    Task<Food?> IBaseRepository<Food>.FindByIdAsync(int id, CancellationToken ct) =>
        base.FindByIdAsync(id, ct);
}
