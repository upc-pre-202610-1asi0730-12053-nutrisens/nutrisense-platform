using Nutrisense.Nutrisense.Platform.ActivityWearable.Application.QueryServices;
using Nutrisense.Nutrisense.Platform.ActivityWearable.Domain.Model.Aggregates;
using Nutrisense.Nutrisense.Platform.ActivityWearable.Domain.Model.Queries;
using Nutrisense.Nutrisense.Platform.ActivityWearable.Domain.Repositories;

namespace Nutrisense.Nutrisense.Platform.ActivityWearable.Application.Internal.QueryServices;

/// <summary>Implementation of <see cref="IActivityLogQueryService"/>. Reads activity logs and computes daily summaries from the repository.</summary>
public class ActivityLogQueryService(IActivityLogRepository repository) : IActivityLogQueryService
{
    /// <summary>Retrieves a user's activity logs within the optional date range.</summary>
    /// <param name="query">The query carrying the user and optional date bounds.</param>
    /// <param name="ct">Token to observe for cancellation.</param>
    /// <returns>The matching activity logs.</returns>
    public async Task<IEnumerable<ActivityLog>> Handle(GetActivityLogsByUserQuery query, CancellationToken ct = default) =>
        await repository.FindByUserAsync(query.UserId, query.FromDate, query.ToDate, ct);

    /// <summary>Aggregates a user's activities for a single day into total calories, total duration and activity count.</summary>
    /// <param name="query">The query carrying the user and date.</param>
    /// <param name="ct">Token to observe for cancellation.</param>
    /// <returns>The computed daily activity summary.</returns>
    public async Task<DailyActivitySummary> Handle(GetDailyActivitySummaryQuery query, CancellationToken ct = default)
    {
        var logs = await repository.FindByUserAndDateAsync(query.UserId, query.Date, ct);
        var list = logs.ToList();
        return new DailyActivitySummary(
            query.Date,
            list.Sum(l => l.CaloriesBurned),
            list.Sum(l => l.DurationMinutes),
            list.Count);
    }
}
