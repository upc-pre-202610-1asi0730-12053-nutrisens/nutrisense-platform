using Nutrisense.Nutrisense.Platform.Shared.Domain.Model.Events;

namespace Nutrisense.Nutrisense.Platform.NutritionTracking.Domain.Model.Events;

// Integration event — consumed by Analytics and Smart Recommendations BCs.
public record ConsumptionUpdated(
    int UserId,
    DateOnly Date,
    decimal TotalCalories,
    decimal ProteinG,
    decimal CarbsG,
    decimal FatG,
    decimal FiberG) : DomainEventBase;
