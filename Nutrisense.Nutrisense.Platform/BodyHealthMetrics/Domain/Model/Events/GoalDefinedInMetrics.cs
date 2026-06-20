using Nutrisense.Nutrisense.Platform.Shared.Domain.Model.Events;

namespace Nutrisense.Nutrisense.Platform.BodyHealthMetrics.Domain.Model.Events;

/// <summary>Domain event raised when a health goal is set or replaced within the BodyHealthMetrics context.</summary>
public record GoalDefinedInMetrics(int UserId, string Goal) : DomainEventBase;
