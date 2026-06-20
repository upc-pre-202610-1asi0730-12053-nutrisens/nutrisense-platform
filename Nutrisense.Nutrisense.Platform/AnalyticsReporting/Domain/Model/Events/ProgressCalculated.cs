using Nutrisense.Nutrisense.Platform.Shared.Domain.Model.Events;

namespace Nutrisense.Nutrisense.Platform.AnalyticsReporting.Domain.Model.Events;

/// <summary>Raised when a user's adherence to their nutrition goals has been recomputed.</summary>
public record ProgressCalculated(int UserId, decimal AdherenceScore) : DomainEventBase;
