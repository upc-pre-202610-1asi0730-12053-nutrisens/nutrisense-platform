using Nutrisense.Nutrisense.Platform.BodyHealthMetrics.Domain.Model.ValueObjects;

namespace Nutrisense.Nutrisense.Platform.BodyHealthMetrics.Domain.Model.Entities;

/// <summary>Timestamped record of a user's body weight at a specific point in time.</summary>
public class WeightLog
{
    public int Id { get; private set; }
    public int UserId { get; private set; }
    public WeightKg WeightKg { get; private set; } = null!;
    public DateTimeOffset LoggedAt { get; private set; }
    public string? Note { get; private set; }

    protected WeightLog() { }

    public WeightLog(int userId, WeightKg weightKg, DateTimeOffset loggedAt, string? note)
    {
        UserId = userId;
        WeightKg = weightKg;
        LoggedAt = loggedAt;
        Note = note;
    }
}
