namespace Nutrisense.Nutrisense.Platform.AnalyticsReporting.Domain.Model.Queries;

/// <summary>Request for a user's current and longest logging streak data.</summary>
public record GetStreakQuery(int UserId);
