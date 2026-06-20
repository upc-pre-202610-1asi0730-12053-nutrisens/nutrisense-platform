using Nutrisense.Nutrisense.Platform.Shared.Domain.Model.Events;

namespace Nutrisense.Nutrisense.Platform.BodyHealthMetrics.Domain.Model.Events;

/// <summary>Domain event raised after a user's basal metabolic rate is computed.</summary>
public record BmrCalculated(int UserId, decimal Bmr) : DomainEventBase;
