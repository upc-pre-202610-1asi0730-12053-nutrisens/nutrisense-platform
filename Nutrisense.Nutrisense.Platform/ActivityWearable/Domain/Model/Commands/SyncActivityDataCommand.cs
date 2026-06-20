namespace Nutrisense.Nutrisense.Platform.ActivityWearable.Domain.Model.Commands;

/// <summary>Represents the intention to import activity data for a wearable connection from its provider.</summary>
/// <param name="WearableConnectionId">Identifier of the wearable connection to sync.</param>
public record SyncActivityDataCommand(int WearableConnectionId);
