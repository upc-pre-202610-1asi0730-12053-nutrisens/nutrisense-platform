using Nutrisense.Nutrisense.Platform.Shared.Domain.Model.Events;

namespace Nutrisense.Nutrisense.Platform.BodyHealthMetrics.Domain.Model.Events;

/// <summary>Domain event raised when a user's initial body-metrics profile is created.</summary>
public record BodyMetricsRegistered(int UserId) : DomainEventBase;
