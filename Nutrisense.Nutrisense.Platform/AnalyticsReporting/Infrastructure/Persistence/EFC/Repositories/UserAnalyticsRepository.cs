using Microsoft.EntityFrameworkCore;
using Nutrisense.Nutrisense.Platform.AnalyticsReporting.Domain.Model.Aggregates;
using Nutrisense.Nutrisense.Platform.AnalyticsReporting.Domain.Model.Entities;
using Nutrisense.Nutrisense.Platform.AnalyticsReporting.Domain.Repositories;
using Nutrisense.Nutrisense.Platform.Shared.Infrastructure.Persistence.EFC.Configuration;
using Nutrisense.Nutrisense.Platform.Shared.Infrastructure.Persistence.EFC.Repositories;

namespace Nutrisense.Nutrisense.Platform.AnalyticsReporting.Infrastructure.Persistence.EFC.Repositories;

/// <summary>EF Core repository for loading and provisioning user analytics aggregates and snapshots.</summary>
public class UserAnalyticsRepository(AppDbContext context)
    : BaseRepository<UserAnalytics>(context), IUserAnalyticsRepository
{
    public async Task<UserAnalytics?> FindByUserIdAsync(int userId, CancellationToken ct = default)
        => await Context.Set<UserAnalytics>()
            .Include(u => u.ProgressSnapshots)
            .FirstOrDefaultAsync(u => u.UserId == userId, ct);

    public async Task<UserAnalytics> FindOrCreateAsync(int userId, CancellationToken ct = default)
    {
        var existing = await FindByUserIdAsync(userId, ct);
        if (existing is not null) return existing;

        var analytics = new UserAnalytics(userId);
        await AddAsync(analytics, ct);
        return analytics;
    }

    public async Task<IEnumerable<ProgressSnapshot>> GetProgressSnapshotsAsync(
        int userId, DateOnly from, DateOnly to, CancellationToken ct = default)
    {
        var analytics = await Context.Set<UserAnalytics>()
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.UserId == userId, ct);

        if (analytics is null) return [];

        return await Context.Set<ProgressSnapshot>()
            .Where(s => s.UserAnalyticsId == analytics.Id
                        && s.Date >= from
                        && s.Date <= to)
            .OrderBy(s => s.Date)
            .ToListAsync(ct);
    }
}
