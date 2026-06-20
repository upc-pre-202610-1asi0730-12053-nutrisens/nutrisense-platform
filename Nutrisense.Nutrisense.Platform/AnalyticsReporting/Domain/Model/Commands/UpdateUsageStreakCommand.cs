namespace Nutrisense.Nutrisense.Platform.AnalyticsReporting.Domain.Model.Commands;

/// <summary>Request to advance or reset a user's consecutive-day logging streak.</summary>
public record UpdateUsageStreakCommand(int UserId);
