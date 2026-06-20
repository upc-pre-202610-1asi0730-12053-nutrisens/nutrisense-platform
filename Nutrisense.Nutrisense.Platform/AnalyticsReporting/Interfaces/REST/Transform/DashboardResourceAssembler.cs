using Nutrisense.Nutrisense.Platform.AnalyticsReporting.Application.Internal;
using Nutrisense.Nutrisense.Platform.AnalyticsReporting.Interfaces.REST.Resources;

namespace Nutrisense.Nutrisense.Platform.AnalyticsReporting.Interfaces.REST.Transform;

/// <summary>Maps dashboard read models to their API resource representation.</summary>
public static class DashboardResourceAssembler
{
    public static DashboardResource ToResourceFromData(DashboardData data)
        => new(
            data.UserId,
            data.Date.ToString("yyyy-MM-dd"),
            data.TotalCalories,
            data.TotalProteinG,
            data.TotalCarbsG,
            data.TotalFatG,
            data.ActiveCaloriesBurned,
            data.AdherenceScore,
            data.CurrentStreak,
            data.WeeklyCompletionRate);
}
