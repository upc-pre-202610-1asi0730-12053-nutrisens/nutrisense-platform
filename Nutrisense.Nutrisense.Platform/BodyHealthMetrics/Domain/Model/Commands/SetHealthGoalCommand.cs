namespace Nutrisense.Nutrisense.Platform.BodyHealthMetrics.Domain.Model.Commands;

/// <summary>Command to define or replace a user's active health goal.</summary>
public record SetHealthGoalCommand(int UserId, string Goal, decimal TargetWeightKg, decimal WeeklyRateKg);
