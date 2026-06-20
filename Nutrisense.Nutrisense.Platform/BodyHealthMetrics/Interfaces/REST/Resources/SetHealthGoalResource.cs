namespace Nutrisense.Nutrisense.Platform.BodyHealthMetrics.Interfaces.REST.Resources;

/// <summary>Request payload for setting a user's health goal.</summary>
public record SetHealthGoalResource(
    /// <summary>Goal type: Lose, Maintain, Gain.</summary>
    string Goal,
    /// <summary>Target weight in kilograms.</summary>
    decimal TargetWeightKg,
    /// <summary>Weekly weight change rate in kilograms (positive for gain, negative for loss).</summary>
    decimal WeeklyRateKg);
