namespace Nutrisense.Nutrisense.Platform.BodyHealthMetrics.Domain.Model.Commands;

/// <summary>Command to record a new waist/neck circumference measurement for a user.</summary>
public record RegisterBodyMeasurementCommand(int UserId, decimal WaistCm, decimal NeckCm);
