namespace Nutrisense.Nutrisense.Platform.ActivityWearable.Interfaces.REST.Resources;

/// <summary>Input resource for toggling automatic syncing on a wearable connection.</summary>
/// <param name="Enabled">True to enable automatic syncing; false to disable it.</param>
public record SetAutoSyncResource(bool Enabled);
