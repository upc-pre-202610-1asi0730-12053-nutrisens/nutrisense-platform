namespace Nutrisense.Nutrisense.Platform.ActivityWearable.Domain.Model.Commands;

/// <summary>Represents the intention to recompute the caloric balance for a user on a given day.</summary>
/// <param name="UserId">Identifier of the user whose balance is adjusted.</param>
/// <param name="Date">Calendar day for which the balance is adjusted.</param>
public record AdjustCaloricBalanceCommand(int UserId, DateOnly Date);
