namespace Nutrisense.Nutrisense.Platform.AnalyticsReporting.Domain.Services;

/// <summary>Domain service that derives updated streak counts from a user's last log date.</summary>
public interface IStreakCalculator
{
    (int CurrentStreak, int LongestStreak) Calculate(
        DateOnly? lastLogDate, int currentStreak, int longestStreak, DateOnly today);
}
