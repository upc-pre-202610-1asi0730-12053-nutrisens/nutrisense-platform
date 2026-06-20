using Nutrisense.Nutrisense.Platform.Shared.Domain.Model.Events;

namespace Nutrisense.Nutrisense.Platform.BodyHealthMetrics.Domain.Model.Events;

/// <summary>Domain event raised when a user logs a new body weight.</summary>
public record WeightUpdated(int UserId, decimal NewWeightKg) : DomainEventBase;
