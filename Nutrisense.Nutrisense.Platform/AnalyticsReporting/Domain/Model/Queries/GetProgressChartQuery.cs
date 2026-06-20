namespace Nutrisense.Nutrisense.Platform.AnalyticsReporting.Domain.Model.Queries;

/// <summary>Request for a user's progress snapshots over a date range, for charting.</summary>
public record GetProgressChartQuery(int UserId, DateOnly From, DateOnly To);
