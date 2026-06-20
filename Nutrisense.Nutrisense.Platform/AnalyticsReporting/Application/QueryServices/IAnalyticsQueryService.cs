using Nutrisense.Nutrisense.Platform.AnalyticsReporting.Application.Internal;
using Nutrisense.Nutrisense.Platform.AnalyticsReporting.Domain.Model.Queries;

namespace Nutrisense.Nutrisense.Platform.AnalyticsReporting.Application.QueryServices;

/// <summary>Application service answering analytics read queries for dashboards, charts, and streaks.</summary>
public interface IAnalyticsQueryService
{
    Task<DashboardData> Handle(GetDashboardQuery query, CancellationToken ct = default);
    Task<ProgressChartData> Handle(GetProgressChartQuery query, CancellationToken ct = default);
    Task<StreakData> Handle(GetStreakQuery query, CancellationToken ct = default);
}
