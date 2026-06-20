namespace Nutrisense.Nutrisense.Platform.ActivityWearable.Interfaces.REST.Resources;

/// <summary>Output resource representing a user's aggregated activity for a single day.</summary>
/// <param name="Date">Calendar day the summary covers (yyyy-MM-dd).</param>
/// <param name="TotalCaloriesBurned">Sum of calories burned that day, in kilocalories.</param>
/// <param name="TotalDurationMinutes">Sum of activity durations that day, in minutes.</param>
/// <param name="ActivityCount">Number of activities recorded that day.</param>
public record DailyActivitySummaryResource(
    string Date,
    decimal TotalCaloriesBurned,
    int TotalDurationMinutes,
    int ActivityCount);
