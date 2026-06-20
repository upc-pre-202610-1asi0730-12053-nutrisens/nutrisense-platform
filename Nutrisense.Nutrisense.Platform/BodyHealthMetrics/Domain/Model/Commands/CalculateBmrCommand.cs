namespace Nutrisense.Nutrisense.Platform.BodyHealthMetrics.Domain.Model.Commands;

/// <summary>Command to trigger a BMR recalculation for a user.</summary>
public record CalculateBmrCommand(int UserId);
