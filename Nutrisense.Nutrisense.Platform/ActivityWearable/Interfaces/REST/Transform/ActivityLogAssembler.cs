using Nutrisense.Nutrisense.Platform.ActivityWearable.Domain.Model.Aggregates;
using Nutrisense.Nutrisense.Platform.ActivityWearable.Domain.Model.Commands;
using Nutrisense.Nutrisense.Platform.ActivityWearable.Domain.Model.Queries;
using Nutrisense.Nutrisense.Platform.ActivityWearable.Interfaces.REST.Resources;

namespace Nutrisense.Nutrisense.Platform.ActivityWearable.Interfaces.REST.Transform;

/// <summary>Converts between activity-log REST resources and the corresponding commands, aggregates and read models.</summary>
public static class ActivityLogAssembler
{
    /// <summary>Maps a manual-activity input resource to its command, parsing the date string.</summary>
    /// <param name="resource">The input resource to convert.</param>
    /// <returns>The equivalent <see cref="LogManualActivityCommand"/>.</returns>
    public static LogManualActivityCommand ToCommand(LogManualActivityResource resource) =>
        new(resource.UserId,
            DateOnly.ParseExact(resource.Date, "yyyy-MM-dd", null),
            resource.ActivityType,
            resource.DurationMinutes,
            resource.Intensity,
            resource.CaloriesBurned);

    /// <summary>Maps an <see cref="ActivityLog"/> aggregate to its output resource, formatting the date.</summary>
    /// <param name="log">The aggregate to convert.</param>
    /// <returns>The equivalent <see cref="ActivityLogResource"/>.</returns>
    public static ActivityLogResource ToResource(ActivityLog log) =>
        new(log.Id, log.UserId, log.Date.ToString("yyyy-MM-dd"), log.ActivityType,
            log.DurationMinutes, log.Intensity, log.CaloriesBurned, log.Source, log.LoggedAt);

    /// <summary>Maps a <see cref="DailyActivitySummary"/> read model to its output resource, formatting the date.</summary>
    /// <param name="summary">The read model to convert.</param>
    /// <returns>The equivalent <see cref="DailyActivitySummaryResource"/>.</returns>
    public static DailyActivitySummaryResource ToSummaryResource(DailyActivitySummary summary) =>
        new(summary.Date.ToString("yyyy-MM-dd"), summary.TotalCaloriesBurned,
            summary.TotalDurationMinutes, summary.ActivityCount);
}
