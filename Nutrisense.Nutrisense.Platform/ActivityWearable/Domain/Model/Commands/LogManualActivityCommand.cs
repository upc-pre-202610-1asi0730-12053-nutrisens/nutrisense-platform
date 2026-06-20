namespace Nutrisense.Nutrisense.Platform.ActivityWearable.Domain.Model.Commands;

/// <summary>Represents the intention to log a manual activity for a user.</summary>
/// <param name="UserId">Identifier of the user the activity belongs to.</param>
/// <param name="Date">Calendar day on which the activity took place.</param>
/// <param name="ActivityType">Kind of activity performed.</param>
/// <param name="DurationMinutes">Duration of the activity in minutes.</param>
/// <param name="Intensity">Effort level: "low", "medium" or "high".</param>
/// <param name="CaloriesBurned">Energy expended by the activity, in kilocalories.</param>
public record LogManualActivityCommand(
    int UserId,
    DateOnly Date,
    string ActivityType,
    int DurationMinutes,
    string Intensity,
    decimal CaloriesBurned);
