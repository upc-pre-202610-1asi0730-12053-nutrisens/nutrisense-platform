namespace Nutrisense.Nutrisense.Platform.BodyHealthMetrics.Domain.Model.Queries;

/// <summary>Query to retrieve a user's weight logs within an optional date range.</summary>
public record GetWeightHistoryByUserIdQuery(
    int UserId,
    DateTimeOffset? From,
    DateTimeOffset? To);
