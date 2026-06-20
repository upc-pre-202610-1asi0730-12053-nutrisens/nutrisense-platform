using Nutrisense.Nutrisense.Platform.AnalyticsReporting.Application.QueryServices;
using Nutrisense.Nutrisense.Platform.AnalyticsReporting.Domain.Model.Queries;
using Nutrisense.Nutrisense.Platform.AnalyticsReporting.Domain.Repositories;
using Nutrisense.Nutrisense.Platform.ActivityWearable.Interfaces.Acl;
using Nutrisense.Nutrisense.Platform.NutritionTracking.Interfaces.Acl;

namespace Nutrisense.Nutrisense.Platform.AnalyticsReporting.Application.Internal.QueryServices;

/// <summary>Assembles analytics read models for dashboards, progress charts, and streaks.</summary>
public class AnalyticsQueryService(
    IUserAnalyticsRepository userAnalyticsRepository,
    INutritionTrackingContextFacade nutritionTrackingFacade,
    IActivityWearableContextFacade activityWearableFacade) : IAnalyticsQueryService
{
    public async Task<DashboardData> Handle(GetDashboardQuery query, CancellationToken ct = default)
    {
        var analytics = await userAnalyticsRepository.FindByUserIdAsync(query.UserId, ct);
        var nutrition = await nutritionTrackingFacade.GetDailyMacroSummary(query.UserId, query.Date, ct);
        var activeCalories = await activityWearableFacade.GetDailyCaloriesBurned(query.UserId, query.Date, ct);

        return new DashboardData(
            UserId: query.UserId,
            Date: query.Date,
            TotalCalories: nutrition?.TotalCalories ?? 0,
            TotalProteinG: nutrition?.TotalProteinG ?? 0,
            TotalCarbsG: nutrition?.TotalCarbsG ?? 0,
            TotalFatG: nutrition?.TotalFatG ?? 0,
            ActiveCaloriesBurned: activeCalories,
            AdherenceScore: analytics?.LastAdherenceScore,
            CurrentStreak: analytics?.CurrentStreak ?? 0,
            WeeklyCompletionRate: analytics?.WeeklyCompletionRate ?? 0);
    }

    public async Task<ProgressChartData> Handle(GetProgressChartQuery query, CancellationToken ct = default)
    {
        var snapshots = await userAnalyticsRepository.GetProgressSnapshotsAsync(
            query.UserId, query.From, query.To, ct);

        var dtos = snapshots.Select(s => new ProgressSnapshotDto(s.Date, s.TotalCalories, s.AdherenceScore));

        return new ProgressChartData(query.UserId, query.From, query.To, dtos);
    }

    public async Task<StreakData> Handle(GetStreakQuery query, CancellationToken ct = default)
    {
        var analytics = await userAnalyticsRepository.FindByUserIdAsync(query.UserId, ct);

        if (analytics is null)
            return new StreakData(query.UserId, 0, 0, null, 0, []);

        // Complete days within the last week, so the client can map them onto its
        // local Mon–Sun calendar week. Latest snapshot per day wins.
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var weekAgo = today.AddDays(-6);
        var completedDates = analytics.ProgressSnapshots
            .Where(s => s.Date >= weekAgo && s.Date <= today)
            .GroupBy(s => s.Date)
            .Where(g => g.OrderByDescending(s => s.CreatedAt).First().IsComplete)
            .Select(g => g.Key)
            .OrderBy(d => d)
            .ToList();

        return new StreakData(
            query.UserId,
            analytics.CurrentStreak,
            analytics.LongestStreak,
            analytics.LastLogDate,
            analytics.WeeklyCompletionRate,
            completedDates);
    }
}
