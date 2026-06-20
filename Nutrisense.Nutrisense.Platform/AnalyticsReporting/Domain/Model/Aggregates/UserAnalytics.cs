using Nutrisense.Nutrisense.Platform.AnalyticsReporting.Domain.Model.Entities;

namespace Nutrisense.Nutrisense.Platform.AnalyticsReporting.Domain.Model.Aggregates;

/// <summary>Aggregate root holding a user's streak, adherence, and progress history.</summary>
public partial class UserAnalytics
{
    public int Id { get; private set; }

    /// <summary>Reference to IAM.User — no FK, cross-BC boundary.</summary>
    public int UserId { get; private set; }

    public int CurrentStreak { get; private set; }
    public int LongestStreak { get; private set; }
    public DateOnly? LastLogDate { get; private set; }
    public decimal WeeklyCompletionRate { get; private set; }
    public decimal? LastAdherenceScore { get; private set; }
    public DateTimeOffset? LastProgressCalculatedAt { get; private set; }

    private readonly List<ProgressSnapshot> _progressSnapshots = [];
    public IReadOnlyCollection<ProgressSnapshot> ProgressSnapshots => _progressSnapshots.AsReadOnly();

    protected UserAnalytics() { }

    public UserAnalytics(int userId)
    {
        UserId = userId;
    }

    public void UpdateStreak(DateOnly today)
    {
        if (LastLogDate == today) return;

        if (LastLogDate == today.AddDays(-1))
            CurrentStreak++;
        else
            CurrentStreak = 1;

        if (CurrentStreak > LongestStreak)
            LongestStreak = CurrentStreak;

        LastLogDate = today;
    }

    public void UpdateAdherence(decimal score)
    {
        LastAdherenceScore = score;
        LastProgressCalculatedAt = DateTimeOffset.UtcNow;
    }

    public void AddProgressSnapshot(ProgressSnapshot snapshot)
    {
        _progressSnapshots.Add(snapshot);
        var cutoff = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(-90);
        _progressSnapshots.RemoveAll(s => s.Date < cutoff);
    }

    public void UpdateWeeklyCompletionRate(decimal rate)
    {
        WeeklyCompletionRate = Math.Clamp(rate, 0, 1);
    }
}
