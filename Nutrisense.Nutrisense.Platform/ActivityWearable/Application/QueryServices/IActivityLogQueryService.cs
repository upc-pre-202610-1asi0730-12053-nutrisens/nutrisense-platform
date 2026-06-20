using Nutrisense.Nutrisense.Platform.ActivityWearable.Domain.Model.Aggregates;
using Nutrisense.Nutrisense.Platform.ActivityWearable.Domain.Model.Queries;

namespace Nutrisense.Nutrisense.Platform.ActivityWearable.Application.QueryServices;

/// <summary>Contract for handling activity-log read operations.</summary>
public interface IActivityLogQueryService
{
    /// <summary>Retrieves a user's activity logs, optionally bounded by a date range.</summary>
    /// <param name="query">The query carrying the user and optional date bounds.</param>
    /// <param name="ct">Token to observe for cancellation.</param>
    /// <returns>The matching activity logs.</returns>
    Task<IEnumerable<ActivityLog>> Handle(GetActivityLogsByUserQuery query, CancellationToken ct = default);

    /// <summary>Retrieves the aggregated daily activity summary for a user on a given day.</summary>
    /// <param name="query">The query carrying the user and date.</param>
    /// <param name="ct">Token to observe for cancellation.</param>
    /// <returns>The daily activity summary.</returns>
    Task<DailyActivitySummary> Handle(GetDailyActivitySummaryQuery query, CancellationToken ct = default);
}
