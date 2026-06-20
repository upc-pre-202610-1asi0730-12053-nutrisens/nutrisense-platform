using Nutrisense.Nutrisense.Platform.Shared.Domain.Model.Events;

namespace Nutrisense.Nutrisense.Platform.ActivityWearable.Domain.Model.Events;

/// <summary>Domain event published when a user successfully connects a wearable device.</summary>
/// <param name="UserId">Identifier of the user who connected the device.</param>
/// <param name="WearableConnectionId">Identifier of the newly created wearable connection.</param>
/// <param name="Provider">External provider the device was connected to (e.g. "google-fit").</param>
public record DeviceConnected(int UserId, int WearableConnectionId, string Provider) : DomainEventBase;
