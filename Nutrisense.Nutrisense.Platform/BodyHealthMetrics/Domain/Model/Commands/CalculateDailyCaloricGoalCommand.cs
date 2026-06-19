namespace Nutrisense.Nutrisense.Platform.BodyHealthMetrics.Domain.Model.Commands;

/// <summary>Command to (re)calculate and persist the daily caloric and macro targets for a user.</summary>
public record CalculateDailyCaloricGoalCommand(int UserId);
