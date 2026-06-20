using Microsoft.EntityFrameworkCore;
using Nutrisense.Nutrisense.Platform.NutritionTracking.Domain.Model.Aggregates;
using Nutrisense.Nutrisense.Platform.NutritionTracking.Domain.Model.ValueObjects;
using Nutrisense.Nutrisense.Platform.NutritionTracking.Domain.Repositories;
using Nutrisense.Nutrisense.Platform.Shared.Domain.Repositories;
using Nutrisense.Nutrisense.Platform.Shared.Infrastructure.Persistence.EFC.Configuration;
using Nutrisense.Nutrisense.Platform.Shared.Infrastructure.Persistence.EFC.Repositories;

namespace Nutrisense.Nutrisense.Platform.NutritionTracking.Infrastructure.Persistence.EFC.Repositories;

public class NutritionLogRepository(AppDbContext context) : BaseRepository<NutritionLog>(context), INutritionLogRepository
{
    public async Task<IEnumerable<NutritionLog>> FindByUserAndDateAsync(int userId, DateOnly date, CancellationToken ct = default) =>
        await Context.Set<NutritionLog>()
            .Where(n => n.UserId == userId && n.Date == date)
            .ToListAsync(ct);

    public async Task<IEnumerable<NutritionLog>> FindByUserAsync(int userId, DateOnly? from, DateOnly? to, CancellationToken ct = default)
    {
        var query = Context.Set<NutritionLog>().Where(n => n.UserId == userId);
        if (from.HasValue) query = query.Where(n => n.Date >= from.Value);
        if (to.HasValue) query = query.Where(n => n.Date <= to.Value);
        return await query.OrderByDescending(n => n.Date).ToListAsync(ct);
    }

    public async Task<DailyMacroSummary> GetDailyMacroSummaryAsync(int userId, DateOnly date, CancellationToken ct = default)
    {
        var logs = await Context.Set<NutritionLog>()
            .Where(n => n.UserId == userId && n.Date == date)
            .ToListAsync(ct);

        return new DailyMacroSummary(
            date,
            logs.Sum(l => l.Calories),
            logs.Sum(l => l.ProteinG),
            logs.Sum(l => l.CarbsG),
            logs.Sum(l => l.FatG),
            logs.Sum(l => l.FiberG),
            logs.Count);
    }

    // Route interface call through base
    Task<NutritionLog?> IBaseRepository<NutritionLog>.FindByIdAsync(int id, CancellationToken ct) =>
        base.FindByIdAsync(id, ct);
}
