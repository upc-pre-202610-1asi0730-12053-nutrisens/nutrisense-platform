using Nutrisense.Nutrisense.Platform.Shared.Domain.Model.Events;

namespace Nutrisense.Nutrisense.Platform.ActivityWearable.Domain.Model.Events;

/// <summary>
/// Domain event published when a user's caloric balance for a day has been adjusted.
/// Integration event — consumed by Analytics &amp; Reporting BC.
/// </summary>
/// <param name="UserId">Identifier of the user the balance belongs to.</param>
/// <param name="Date">Calendar day the balance was adjusted for.</param>
/// <param name="BalanceKcal">Resulting caloric balance for the day, in kilocalories.</param>
public record CaloricBalanceAdjusted(int UserId, DateOnly Date, decimal BalanceKcal) : DomainEventBase;
