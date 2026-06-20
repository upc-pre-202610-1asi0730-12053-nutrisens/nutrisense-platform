namespace Nutrisense.Nutrisense.Platform.ActivityWearable.Domain.Model.Commands;

/// <summary>Represents the intention to disconnect a previously connected wearable device.</summary>
/// <param name="WearableConnectionId">Identifier of the wearable connection to disconnect.</param>
public record DisconnectDeviceCommand(int WearableConnectionId);
