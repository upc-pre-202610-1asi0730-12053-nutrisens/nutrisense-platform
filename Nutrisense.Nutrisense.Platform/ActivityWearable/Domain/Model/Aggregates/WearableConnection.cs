using Nutrisense.Nutrisense.Platform.ActivityWearable.Domain.Model.Commands;
using Nutrisense.Nutrisense.Platform.ActivityWearable.Domain.Model.ValueObjects;

namespace Nutrisense.Nutrisense.Platform.ActivityWearable.Domain.Model.Aggregates;

/// <summary>
/// Aggregate root representing a user's link to an external wearable provider (e.g. Google Fit).
/// Tracks the authorization lifecycle and the last successful synchronization of activity data.
/// </summary>
public partial class WearableConnection
{
    /// <summary>Unique identifier assigned by the persistence layer.</summary>
    public int Id { get; private set; }

    /// <summary>Reference to IAM.User — no FK, cross-BC boundary.</summary>
    public int UserId { get; private set; }

    /// <summary>External provider this connection authorizes against (e.g. "google-fit").</summary>
    public string Provider { get; private set; } = null!;

    /// <summary>Lifecycle state of the connection: "pending-auth", "connected" or "disconnected".</summary>
    public string Status { get; private set; } = null!;

    /// <summary>Instant (UTC) of the last successful activity sync, or null if never synced.</summary>
    public DateTimeOffset? LastSyncedAt { get; private set; }

    /// <summary>Instant (UTC) at which the user authorized the connection.</summary>
    public DateTimeOffset AuthorizedAt { get; private set; }

    /// <summary>Parameterless constructor required by EF Core for materialization.</summary>
    protected WearableConnection() { }

    /// <summary>Creates a connection in the "pending-auth" state from a connect command.</summary>
    /// <param name="command">The command carrying the user and provider to connect.</param>
    /// <param name="authorizedAt">Instant (UTC) at which the user authorized the connection.</param>
    public WearableConnection(ConnectDeviceCommand command, DateTimeOffset authorizedAt)
    {
        UserId = command.UserId;
        Provider = new WearableProvider(command.Provider).Value;
        Status = new WearableStatus("pending-auth").Value;
        AuthorizedAt = authorizedAt;
    }

    /// <summary>Records a successful sync, stamping the time and transitioning the connection to "connected".</summary>
    /// <param name="syncedAt">Instant (UTC) at which the sync completed.</param>
    public void ApplySync(DateTimeOffset syncedAt)
    {
        LastSyncedAt = syncedAt;
        Status = new WearableStatus("connected").Value;
    }

    /// <summary>Transitions the connection to the "disconnected" state, revoking further syncing.</summary>
    public void ApplyDisconnect()
    {
        Status = new WearableStatus("disconnected").Value;
    }
}
