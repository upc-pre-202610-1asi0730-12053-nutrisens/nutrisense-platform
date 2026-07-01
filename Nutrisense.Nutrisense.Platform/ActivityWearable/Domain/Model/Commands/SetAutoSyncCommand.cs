namespace Nutrisense.Nutrisense.Platform.ActivityWearable.Domain.Model.Commands;

/// <summary>Represents the intention to enable or disable automatic syncing for a wearable connection.</summary>
/// <param name="WearableConnectionId">Identifier of the wearable connection to update.</param>
/// <param name="Enabled">True to enable automatic syncing; false to disable it.</param>
public record SetAutoSyncCommand(int WearableConnectionId, bool Enabled);
