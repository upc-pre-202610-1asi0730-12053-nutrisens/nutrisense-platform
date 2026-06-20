namespace Nutrisense.Nutrisense.Platform.ActivityWearable.Interfaces.REST.Resources;

/// <summary>Input resource for logging a manual activity.</summary>
/// <param name="UserId">Identifier of the user the activity belongs to.</param>
/// <param name="Date">Calendar day of the activity (yyyy-MM-dd).</param>
/// <param name="ActivityType">Kind of activity performed.</param>
/// <param name="DurationMinutes">Duration of the activity in minutes.</param>
/// <param name="Intensity">Effort level: "low", "medium" or "high".</param>
/// <param name="CaloriesBurned">Energy expended by the activity, in kilocalories.</param>
public record LogManualActivityResource(
    int UserId,
    string Date,
    string ActivityType,
    int DurationMinutes,
    string Intensity,
    decimal CaloriesBurned);
