namespace Nutrisense.Nutrisense.Platform.ActivityWearable.Domain.Model.Queries;

/// <summary>Represents the intention to retrieve a user's activity logs, optionally bounded by a date range.</summary>
/// <param name="UserId">Identifier of the user whose activity logs are requested.</param>
/// <param name="FromDate">Inclusive lower bound of the date range, or null for no lower bound.</param>
/// <param name="ToDate">Inclusive upper bound of the date range, or null for no upper bound.</param>
public record GetActivityLogsByUserQuery(int UserId, DateOnly? FromDate, DateOnly? ToDate);
