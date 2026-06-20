namespace Nutrisense.Nutrisense.Platform.BodyHealthMetrics.Interfaces.REST.Resources;

/// <summary>Request payload for registering a user's initial body-metrics profile.</summary>
public record RegisterBodyMetricsResource(
    /// <summary>Unique user identifier.</summary>
    int UserId,
    /// <summary>Height in centimeters.</summary>
    decimal HeightCm,
    /// <summary>Current weight in kilograms.</summary>
    decimal WeightKg,
    /// <summary>User's date of birth in ISO 8601 format (yyyy-MM-dd).</summary>
    string DateOfBirth,
    /// <summary>Biological sex: 'M' or 'F'.</summary>
    string BiologicalSex,
    /// <summary>Activity level: Sedentary, Light, Moderate, Active, VeryActive.</summary>
    string ActivityLevel,
    /// <summary>Optional goal type: Lose, Maintain, Gain. If not provided, no goal is set.</summary>
    string? Goal = null,
    /// <summary>Weekly weight change rate in kilograms. Required if Goal is provided.</summary>
    decimal? WeeklyRateKg = null);
