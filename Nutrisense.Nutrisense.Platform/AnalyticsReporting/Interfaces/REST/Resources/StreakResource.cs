namespace Nutrisense.Nutrisense.Platform.AnalyticsReporting.Interfaces.REST.Resources;

/// <summary>API response exposing a user's streak counts and recently completed days.</summary>
public record StreakResource(
    /// <summary>The user's unique identifier.</summary>
    int UserId,
    /// <summary>The number of consecutive days the user has maintained their streak.</summary>
    int CurrentStreak,
    /// <summary>The longest streak the user has achieved historically.</summary>
    int LongestStreak,
    /// <summary>The most recent date the user completed their goal in ISO 8601 format (yyyy-MM-dd), or null if never completed.</summary>
    string? LastLogDate,
    /// <summary>The percentage of days the user completed their goals in the current week (0-1).</summary>
    decimal WeeklyCompletionRate,
    /// <summary>List of dates when the user completed their goals in ISO 8601 format (yyyy-MM-dd).</summary>
    IEnumerable<string> CompletedDates);
