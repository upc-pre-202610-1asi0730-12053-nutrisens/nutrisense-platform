using Nutrisense.Nutrisense.Platform.BodyHealthMetrics.Domain.Model.Aggregates;
using Nutrisense.Nutrisense.Platform.BodyHealthMetrics.Domain.Model.Entities;
using Nutrisense.Nutrisense.Platform.Shared.Domain.Repositories;

namespace Nutrisense.Nutrisense.Platform.BodyHealthMetrics.Domain.Repositories;

/// <summary>Persistence contract for the BodyMetrics aggregate and its related child entities.</summary>
public interface IBodyMetricsRepository : IBaseRepository<BodyMetrics>
{
    Task<BodyMetrics?> FindByUserIdAsync(int userId, CancellationToken ct = default);
    Task<UserGoal?> FindActiveGoalByUserIdAsync(int userId, CancellationToken ct = default);
    Task<IEnumerable<WeightLog>> FindWeightHistoryByUserIdAsync(int userId, DateTimeOffset? from, DateTimeOffset? to, CancellationToken ct = default);
}
