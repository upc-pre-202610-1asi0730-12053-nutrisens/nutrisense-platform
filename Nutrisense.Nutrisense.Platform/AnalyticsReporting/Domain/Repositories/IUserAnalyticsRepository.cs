using Nutrisense.Nutrisense.Platform.AnalyticsReporting.Domain.Model.Aggregates;
using Nutrisense.Nutrisense.Platform.AnalyticsReporting.Domain.Model.Entities;
using Nutrisense.Nutrisense.Platform.Shared.Domain.Repositories;

namespace Nutrisense.Nutrisense.Platform.AnalyticsReporting.Domain.Repositories;

/// <summary>Persistence gateway for user analytics aggregates and their progress snapshots.</summary>
public interface IUserAnalyticsRepository : IBaseRepository<UserAnalytics>
{
    Task<UserAnalytics?> FindByUserIdAsync(int userId, CancellationToken ct = default);
    Task<UserAnalytics> FindOrCreateAsync(int userId, CancellationToken ct = default);
    Task<IEnumerable<ProgressSnapshot>> GetProgressSnapshotsAsync(
        int userId, DateOnly from, DateOnly to, CancellationToken ct = default);
}
