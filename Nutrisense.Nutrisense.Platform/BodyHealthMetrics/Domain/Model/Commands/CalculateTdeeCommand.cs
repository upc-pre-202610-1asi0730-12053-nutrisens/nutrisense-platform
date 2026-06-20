namespace Nutrisense.Nutrisense.Platform.BodyHealthMetrics.Domain.Model.Commands;

/// <summary>Command to trigger a TDEE recalculation for a user.</summary>
public record CalculateTdeeCommand(int UserId);
