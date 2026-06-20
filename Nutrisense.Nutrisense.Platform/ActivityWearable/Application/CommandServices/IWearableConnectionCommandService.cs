using Nutrisense.Nutrisense.Platform.ActivityWearable.Application.Errors;
using Nutrisense.Nutrisense.Platform.ActivityWearable.Domain.Model.Aggregates;
using Nutrisense.Nutrisense.Platform.ActivityWearable.Domain.Model.Commands;
using Nutrisense.Nutrisense.Platform.Shared.Application.Patterns;

namespace Nutrisense.Nutrisense.Platform.ActivityWearable.Application.CommandServices;

/// <summary>Contract for handling wearable-connection write operations.</summary>
public interface IWearableConnectionCommandService
{
    /// <summary>Connects a wearable device, authorizing with the provider and importing the initial activity batch.</summary>
    /// <param name="command">The command carrying the user, provider and OAuth code.</param>
    /// <param name="ct">Token to observe for cancellation.</param>
    /// <returns>The created <see cref="WearableConnection"/> on success, or a <see cref="ConnectDeviceError"/> on failure.</returns>
    Task<Result<WearableConnection, ConnectDeviceError>> Handle(ConnectDeviceCommand command, CancellationToken ct = default);

    /// <summary>Imports recent activity data for an existing connection, skipping duplicates.</summary>
    /// <param name="command">The command identifying the connection to sync.</param>
    /// <param name="ct">Token to observe for cancellation.</param>
    /// <returns>The updated <see cref="WearableConnection"/> on success, or a <see cref="SyncActivityDataError"/> on failure.</returns>
    Task<Result<WearableConnection, SyncActivityDataError>> Handle(SyncActivityDataCommand command, CancellationToken ct = default);

    /// <summary>Disconnects an existing wearable connection.</summary>
    /// <param name="command">The command identifying the connection to disconnect.</param>
    /// <param name="ct">Token to observe for cancellation.</param>
    /// <returns>True on success, or a <see cref="DisconnectDeviceError"/> on failure.</returns>
    Task<Result<bool, DisconnectDeviceError>> Handle(DisconnectDeviceCommand command, CancellationToken ct = default);
}
