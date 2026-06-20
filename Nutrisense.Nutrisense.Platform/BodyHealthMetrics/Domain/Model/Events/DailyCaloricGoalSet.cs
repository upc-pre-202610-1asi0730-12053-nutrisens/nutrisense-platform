using Nutrisense.Nutrisense.Platform.Shared.Domain.Model.Events;

namespace Nutrisense.Nutrisense.Platform.BodyHealthMetrics.Domain.Model.Events;

/// <summary>Domain event raised when a user's daily caloric and macro targets are computed and stored.</summary>
public record DailyCaloricGoalSet(
    int UserId,
    int DailyCalories,
    decimal ProteinG,
    decimal CarbsG,
    decimal FatG,
    decimal FiberG) : DomainEventBase;
