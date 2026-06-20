using Nutrisense.Nutrisense.Platform.ActivityWearable.Domain.Model.Commands;
using Nutrisense.Nutrisense.Platform.ActivityWearable.Domain.Model.ValueObjects;

namespace Nutrisense.Nutrisense.Platform.ActivityWearable.Domain.Model.Aggregates;

/// <summary>
/// Aggregate root representing a single physical activity performed by a user on a given day,
/// whether logged manually or imported from a wearable provider. Holds the energy expenditure
/// (calories burned) that drives the daily caloric balance calculation.
/// </summary>
public partial class ActivityLog
{
    /// <summary>Unique identifier assigned by the persistence layer.</summary>
    public int Id { get; private set; }

    /// <summary>Reference to IAM.User — no FK, cross-BC boundary.</summary>
    public int UserId { get; private set; }

    /// <summary>Calendar day on which the activity took place.</summary>
    public DateOnly Date { get; private set; }

    /// <summary>Kind of activity performed (e.g. "walking", "strength-training").</summary>
    public string ActivityType { get; private set; } = null!;

    /// <summary>Duration of the activity in minutes.</summary>
    public int DurationMinutes { get; private set; }

    /// <summary>Effort level of the activity: "low", "medium" or "high".</summary>
    public string Intensity { get; private set; } = null!;

    /// <summary>Energy expended by the activity, in kilocalories.</summary>
    public decimal CaloriesBurned { get; private set; }

    /// <summary>Origin of the record: "manual" or "google-fit".</summary>
    public string Source { get; private set; } = null!;

    /// <summary>Instant (UTC) at which the activity was recorded in the system.</summary>
    public DateTimeOffset LoggedAt { get; private set; }

    /// <summary>Parameterless constructor required by EF Core for materialization.</summary>
    protected ActivityLog() { }

    /// <summary>Creates a manually logged activity from a user-submitted command, marking its source as "manual".</summary>
    /// <param name="command">The command carrying the user-provided activity details.</param>
    public ActivityLog(LogManualActivityCommand command)
    {
        UserId = command.UserId;
        Date = command.Date;
        ActivityType = new ActivityType(command.ActivityType).Value;
        DurationMinutes = command.DurationMinutes;
        Intensity = new Intensity(command.Intensity).Value;
        CaloriesBurned = command.CaloriesBurned;
        Source = new ActivitySource("manual").Value;
        LoggedAt = DateTimeOffset.UtcNow;
    }

    /// <summary>Creates an activity from individual values, used when importing data from a wearable provider.</summary>
    /// <param name="userId">Identifier of the user who owns the activity.</param>
    /// <param name="date">Calendar day on which the activity took place.</param>
    /// <param name="activityType">Kind of activity performed.</param>
    /// <param name="durationMinutes">Duration of the activity in minutes.</param>
    /// <param name="intensity">Effort level: "low", "medium" or "high".</param>
    /// <param name="caloriesBurned">Energy expended by the activity, in kilocalories.</param>
    /// <param name="source">Origin of the record (e.g. "google-fit").</param>
    public ActivityLog(int userId, DateOnly date, string activityType, int durationMinutes,
        string intensity, decimal caloriesBurned, string source)
    {
        UserId = userId;
        Date = date;
        ActivityType = new ActivityType(activityType).Value;
        DurationMinutes = durationMinutes;
        Intensity = new Intensity(intensity).Value;
        CaloriesBurned = caloriesBurned;
        Source = new ActivitySource(source).Value;
        LoggedAt = DateTimeOffset.UtcNow;
    }
}
