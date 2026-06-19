namespace Nutrisense.Nutrisense.Platform.BodyHealthMetrics.Domain.Model.Commands;

/// <summary>Command to log a new weight entry for an existing body-metrics profile.</summary>
public record UpdateWeightCommand(int UserId, decimal WeightKg, string? Note);
