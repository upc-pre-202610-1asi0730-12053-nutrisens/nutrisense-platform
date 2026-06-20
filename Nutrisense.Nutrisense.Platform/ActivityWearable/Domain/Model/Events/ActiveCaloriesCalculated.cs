using Nutrisense.Nutrisense.Platform.Shared.Domain.Model.Events;

namespace Nutrisense.Nutrisense.Platform.ActivityWearable.Domain.Model.Events;

/// <summary>Domain event published when a user's active calories for a day have been recalculated.</summary>
/// <param name="UserId">Identifier of the user the calculation belongs to.</param>
/// <param name="Date">Calendar day the active calories were calculated for.</param>
/// <param name="ActiveCalories">Total active calories burned that day, in kilocalories.</param>
public record ActiveCaloriesCalculated(int UserId, DateOnly Date, decimal ActiveCalories) : DomainEventBase;
