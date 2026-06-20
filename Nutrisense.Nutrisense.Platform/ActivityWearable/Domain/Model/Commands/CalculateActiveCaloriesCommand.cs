namespace Nutrisense.Nutrisense.Platform.ActivityWearable.Domain.Model.Commands;

/// <summary>Represents the intention to calculate the active calories burned by a user on a given day.</summary>
/// <param name="UserId">Identifier of the user whose active calories are calculated.</param>
/// <param name="Date">Calendar day for which active calories are calculated.</param>
public record CalculateActiveCaloriesCommand(int UserId, DateOnly Date);
