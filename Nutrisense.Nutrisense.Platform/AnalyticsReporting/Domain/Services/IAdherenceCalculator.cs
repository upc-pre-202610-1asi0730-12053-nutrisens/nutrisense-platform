using Nutrisense.Nutrisense.Platform.BodyHealthMetrics.Interfaces.Acl;
using Nutrisense.Nutrisense.Platform.NutritionTracking.Interfaces.Acl;

namespace Nutrisense.Nutrisense.Platform.AnalyticsReporting.Domain.Services;

/// <summary>Domain service that scores how closely actual intake matched a user's goals.</summary>
public interface IAdherenceCalculator
{
    decimal Calculate(DailyMacroSummaryItem? actual, UserGoalSummaryItem? goal);
}
