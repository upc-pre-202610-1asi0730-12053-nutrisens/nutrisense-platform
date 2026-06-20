namespace Nutrisense.Nutrisense.Platform.AnalyticsReporting.Domain.Model.Commands;

/// <summary>Request to recompute a user's progress and adherence metrics.</summary>
public record CalculateProgressCommand(int UserId);
