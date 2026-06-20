using Microsoft.EntityFrameworkCore;
using Nutrisense.Nutrisense.Platform.BodyHealthMetrics.Domain.Model.Aggregates;
using Nutrisense.Nutrisense.Platform.BodyHealthMetrics.Domain.Model.Entities;
using Nutrisense.Nutrisense.Platform.BodyHealthMetrics.Domain.Repositories;
using Nutrisense.Nutrisense.Platform.Shared.Infrastructure.Persistence.EFC.Configuration;
using Nutrisense.Nutrisense.Platform.Shared.Infrastructure.Persistence.EFC.Repositories;

namespace Nutrisense.Nutrisense.Platform.BodyHealthMetrics.Infrastructure.Persistence.EFC.Repositories;

/// <summary>EF Core repository implementation for the BodyMetrics aggregate.</summary>
public class BodyMetricsRepository(AppDbContext context) : BaseRepository<BodyMetrics>(context), IBodyMetricsRepository
{
    private IQueryable<BodyMetrics> WithRelations() =>
        Context.Set<BodyMetrics>()
            .Include(b => b.WeightLogs)
            .Include(b => b.BodyMeasurements)
            .Include(b => b.UserGoals);

    public async Task<BodyMetrics?> FindByUserIdAsync(int userId, CancellationToken ct = default) =>
        await WithRelations().FirstOrDefaultAsync(b => b.UserId == userId, ct);

    public async Task<UserGoal?> FindActiveGoalByUserIdAsync(int userId, CancellationToken ct = default) =>
        await Context.Set<UserGoal>().FirstOrDefaultAsync(g => g.UserId == userId && g.Active, ct);

    public async Task<IEnumerable<WeightLog>> FindWeightHistoryByUserIdAsync(
        int userId, DateTimeOffset? from, DateTimeOffset? to, CancellationToken ct = default)
    {
        var query = Context.Set<WeightLog>().Where(w => w.UserId == userId);
        if (from.HasValue) query = query.Where(w => w.LoggedAt >= from.Value);
        if (to.HasValue)   query = query.Where(w => w.LoggedAt <= to.Value);
        return await query.OrderBy(w => w.LoggedAt).ToListAsync(ct);
    }
}
