using Microsoft.EntityFrameworkCore;
using Nutrisense.Nutrisense.Platform.NutritionTracking.Domain.Model.Aggregates;
using Nutrisense.Nutrisense.Platform.NutritionTracking.Domain.Repositories;
using Nutrisense.Nutrisense.Platform.Shared.Domain.Repositories;
using Nutrisense.Nutrisense.Platform.Shared.Infrastructure.Persistence.EFC.Configuration;
using Nutrisense.Nutrisense.Platform.Shared.Infrastructure.Persistence.EFC.Repositories;

namespace Nutrisense.Nutrisense.Platform.NutritionTracking.Infrastructure.Persistence.EFC.Repositories;

public class FoodRepository(AppDbContext context) : BaseRepository<Food>(context), IFoodRepository
{
    /// <summary>Maximum number of search matches returned, ordered by relevance.</summary>
    private const int MaxResults = 25;

    public async Task<Food?> FindByKeyAsync(string key, CancellationToken ct = default) =>
        await Context.Set<Food>().FirstOrDefaultAsync(f => f.Key == key, ct);

    public async Task<IEnumerable<Food>> SearchByNameAsync(string query, string lang, CancellationToken ct = default)
    {
        var lower = (query ?? string.Empty).Trim().ToLower();
        if (lower.Length == 0)
            return [];

        // Rank substring matches: exact name first, then prefix matches, then the rest; within each
        // tier prefer shorter (more specific) names, alphabetically. Capped to keep results focused.
        return await Context.Set<Food>()
            .Where(f => f.NameEn.ToLower().Contains(lower) || f.NameEs.ToLower().Contains(lower))
            .OrderByDescending(f => f.NameEn.ToLower() == lower || f.NameEs.ToLower() == lower)
            .ThenByDescending(f => f.NameEn.ToLower().StartsWith(lower) || f.NameEs.ToLower().StartsWith(lower))
            .ThenBy(f => f.NameEn.Length)
            .ThenBy(f => f.NameEn)
            .Take(MaxResults)
            .ToListAsync(ct);
    }

    public async Task<Food?> FindByExternalIdAsync(string externalId, CancellationToken ct = default) =>
        await Context.Set<Food>().FirstOrDefaultAsync(f => f.ExternalId == externalId, ct);

    // Route interface call through base
    Task<Food?> IBaseRepository<Food>.FindByIdAsync(int id, CancellationToken ct) =>
        base.FindByIdAsync(id, ct);
}
