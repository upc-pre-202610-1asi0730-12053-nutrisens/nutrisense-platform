namespace Nutrisense.Nutrisense.Platform.BodyHealthMetrics.Interfaces.REST.Resources;

/// <summary>Request payload for updating a user's current weight.</summary>
public record UpdateWeightResource(
    /// <summary>New weight value in kilograms.</summary>
    decimal WeightKg,
    /// <summary>Optional user note or comment for this weight update.</summary>
    string? Note);
