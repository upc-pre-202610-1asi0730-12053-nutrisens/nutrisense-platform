namespace Nutrisense.Nutrisense.Platform.BodyHealthMetrics.Interfaces.REST.Resources;

/// <summary>Request payload for recording a waist/neck body measurement.</summary>
public record RegisterBodyMeasurementResource(
    /// <summary>Waist circumference in centimeters.</summary>
    decimal WaistCm,
    /// <summary>Neck circumference in centimeters.</summary>
    decimal NeckCm);
