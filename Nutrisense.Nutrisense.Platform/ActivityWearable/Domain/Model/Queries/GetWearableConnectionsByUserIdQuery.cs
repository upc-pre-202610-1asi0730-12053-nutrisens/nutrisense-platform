namespace Nutrisense.Nutrisense.Platform.ActivityWearable.Domain.Model.Queries;

/// <summary>Represents the intention to retrieve all wearable connections belonging to a user.</summary>
/// <param name="UserId">Identifier of the user whose wearable connections are requested.</param>
public record GetWearableConnectionsByUserIdQuery(int UserId);
