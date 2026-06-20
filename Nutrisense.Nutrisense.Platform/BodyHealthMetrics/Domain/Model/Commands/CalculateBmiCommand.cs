namespace Nutrisense.Nutrisense.Platform.BodyHealthMetrics.Domain.Model.Commands;

/// <summary>Command to trigger a BMI recalculation for a user.</summary>
public record CalculateBmiCommand(int UserId);
