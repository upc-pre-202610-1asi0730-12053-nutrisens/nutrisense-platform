using Nutrisense.Nutrisense.Platform.AnalyticsReporting.Application.Internal;
using Nutrisense.Nutrisense.Platform.AnalyticsReporting.Interfaces.REST.Resources;

namespace Nutrisense.Nutrisense.Platform.AnalyticsReporting.Interfaces.REST.Transform;

/// <summary>Maps streak read models to their API resource representation.</summary>
public static class StreakResourceAssembler
{
    public static StreakResource ToResourceFromData(StreakData data)
        => new(
            data.UserId,
            data.CurrentStreak,
            data.LongestStreak,
            data.LastLogDate?.ToString("yyyy-MM-dd"),
            data.WeeklyCompletionRate,
            data.CompletedDates.Select(d => d.ToString("yyyy-MM-dd")));
}
