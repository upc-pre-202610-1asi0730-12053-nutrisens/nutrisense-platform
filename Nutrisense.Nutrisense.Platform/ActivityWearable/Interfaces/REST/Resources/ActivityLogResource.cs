namespace Nutrisense.Nutrisense.Platform.ActivityWearable.Interfaces.REST.Resources;

/// <summary>Output resource representing an activity log returned to API clients.</summary>
/// <param name="Id">Unique identifier of the activity log.</param>
/// <param name="UserId">Identifier of the user the activity belongs to.</param>
/// <param name="Date">Calendar day of the activity (yyyy-MM-dd).</param>
/// <param name="ActivityType">Kind of activity performed.</param>
/// <param name="DurationMinutes">Duration of the activity in minutes.</param>
/// <param name="Intensity">Effort level: "low", "medium" or "high".</param>
/// <param name="CaloriesBurned">Energy expended by the activity, in kilocalories.</param>
/// <param name="Source">Origin of the record: "manual" or "google-fit".</param>
/// <param name="LoggedAt">Instant (UTC) at which the activity was recorded.</param>
public record ActivityLogResource(
    int Id,
    int UserId,
    string Date,
    string ActivityType,
    int DurationMinutes,
    string Intensity,
    decimal CaloriesBurned,
    string Source,
    DateTimeOffset LoggedAt);
