namespace Nutrisense.Nutrisense.Platform.AnalyticsReporting.Domain.Model.Queries;

/// <summary>Request for a user's dashboard metrics on a specific day.</summary>
public record GetDashboardQuery(int UserId, DateOnly Date);
