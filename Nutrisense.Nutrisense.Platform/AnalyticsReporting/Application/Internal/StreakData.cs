namespace Nutrisense.Nutrisense.Platform.AnalyticsReporting.Application.Internal;

/// <summary>Read model describing a user's streak counts, last log, and recent completed days.</summary>
public record StreakData(
    int UserId,
    int CurrentStreak,
    int LongestStreak,
    DateOnly? LastLogDate,
    decimal WeeklyCompletionRate,
    IReadOnlyList<DateOnly> CompletedDates);
