using Nutrisense.Nutrisense.Platform.ActivityWearable.Domain.Model.Aggregates;
using Nutrisense.Nutrisense.Platform.ActivityWearable.Domain.Model.Commands;
using Nutrisense.Nutrisense.Platform.ActivityWearable.Interfaces.REST.Resources;

namespace Nutrisense.Nutrisense.Platform.ActivityWearable.Interfaces.REST.Transform;

/// <summary>Converts between wearable-connection REST resources and the corresponding command and aggregate.</summary>
public static class WearableConnectionAssembler
{
    /// <summary>Maps a connect-device input resource to its command.</summary>
    /// <param name="resource">The input resource to convert.</param>
    /// <returns>The equivalent <see cref="ConnectDeviceCommand"/>.</returns>
    public static ConnectDeviceCommand ToCommand(ConnectDeviceResource resource) =>
        new(resource.UserId, resource.Provider, resource.OAuthCode);

    /// <summary>Maps a <see cref="WearableConnection"/> aggregate to its output resource.</summary>
    /// <param name="connection">The aggregate to convert.</param>
    /// <returns>The equivalent <see cref="WearableConnectionResource"/>.</returns>
    public static WearableConnectionResource ToResource(WearableConnection connection) =>
        new(connection.Id, connection.UserId, connection.Provider, connection.Status,
            connection.LastSyncedAt, connection.AuthorizedAt, connection.AutoSyncEnabled);
}
