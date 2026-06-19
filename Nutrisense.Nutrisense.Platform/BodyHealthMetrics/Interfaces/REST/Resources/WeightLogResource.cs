namespace Nutrisense.Nutrisense.Platform.BodyHealthMetrics.Interfaces.REST.Resources;

/// <summary>API response representing a single weight-log entry.</summary>
public record WeightLogResource(
    /// <summary>Weight value in kilograms.</summary>
    decimal WeightKg,
    /// <summary>Timestamp when weight was logged in ISO 8601 format.</summary>
    DateTimeOffset LoggedAt,
    /// <summary>Optional user note or comment for this weight entry.</summary>
    string? Note);
