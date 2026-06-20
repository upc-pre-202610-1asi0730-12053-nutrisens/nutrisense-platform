namespace Nutrisense.Nutrisense.Platform.BodyHealthMetrics.Domain.Model.Queries;

/// <summary>Query to retrieve the full body-metrics aggregate for a given user.</summary>
public record GetBodyMetricsByUserIdQuery(int UserId);
