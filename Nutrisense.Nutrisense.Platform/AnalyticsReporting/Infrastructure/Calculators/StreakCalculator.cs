using Nutrisense.Nutrisense.Platform.AnalyticsReporting.Domain.Services;

namespace Nutrisense.Nutrisense.Platform.AnalyticsReporting.Infrastructure.Calculators;

/// <summary>Computes streak progression based on whether logging continued from the previous day.</summary>
public class StreakCalculator : IStreakCalculator
{
    public (int CurrentStreak, int LongestStreak) Calculate(
        DateOnly? lastLogDate, int currentStreak, int longestStreak, DateOnly today)
    {
        if (lastLogDate == today) return (currentStreak, longestStreak);

        int newStreak = lastLogDate == today.AddDays(-1) ? currentStreak + 1 : 1;
        int newLongest = newStreak > longestStreak ? newStreak : longestStreak;

        return (newStreak, newLongest);
    }
}
