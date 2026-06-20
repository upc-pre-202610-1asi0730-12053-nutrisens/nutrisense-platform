using Nutrisense.Nutrisense.Platform.Shared.Domain.Model.Events;

namespace Nutrisense.Nutrisense.Platform.BodyHealthMetrics.Domain.Model.Events;

/// <summary>Domain event raised after a user's total daily energy expenditure is computed.</summary>
public record TdeeCalculated(int UserId, decimal Tdee) : DomainEventBase;
