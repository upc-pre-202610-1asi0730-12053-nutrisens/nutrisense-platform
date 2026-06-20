namespace Nutrisense.Nutrisense.Platform.ActivityWearable.Domain.Model.Queries;

/// <summary>Represents the intention to retrieve the aggregated daily activity summary for a user on a given day.</summary>
/// <param name="UserId">Identifier of the user whose summary is requested.</param>
/// <param name="Date">Calendar day to summarize.</param>
public record GetDailyActivitySummaryQuery(int UserId, DateOnly Date);

/// <summary>Read model aggregating a user's activity for a single day.</summary>
/// <param name="Date">Calendar day the summary covers.</param>
/// <param name="TotalCaloriesBurned">Sum of calories burned across all activities that day, in kilocalories.</param>
/// <param name="TotalDurationMinutes">Sum of activity durations that day, in minutes.</param>
/// <param name="ActivityCount">Number of activities recorded that day.</param>
public record DailyActivitySummary(DateOnly Date, decimal TotalCaloriesBurned, int TotalDurationMinutes, int ActivityCount);
