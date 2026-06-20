using Nutrisense.Nutrisense.Platform.Shared.Domain.Model.Events;

namespace Nutrisense.Nutrisense.Platform.AnalyticsReporting.Domain.Model.Events;

/// <summary>Raised when a user's daily nutrition and activity data has been consolidated for a given day.</summary>
public record UserDataAggregated(int UserId, DateOnly Date) : DomainEventBase;
