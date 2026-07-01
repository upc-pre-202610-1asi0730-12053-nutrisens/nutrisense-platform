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

    /// <summary>Whether the connection should be re-synced automatically (driven by the client on load).</summary>
    public bool AutoSyncEnabled { get; private set; }

    /// <summary>Provider access token used to call the data API. Never exposed to API clients.</summary>
    public string? AccessToken { get; private set; }

    /// <summary>Long-lived provider refresh token. Never exposed to API clients.</summary>
    public string? RefreshToken { get; private set; }

    /// <summary>Instant (UTC) at which <see cref="AccessToken"/> expires, or null if unknown.</summary>
    public DateTimeOffset? TokenExpiresAt { get; private set; }

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

    /// <summary>Stores the access/refresh tokens granted by the provider. Preserves the existing refresh token when the provider returns none (e.g. on a plain refresh).</summary>
    /// <param name="accessToken">The freshly granted access token.</param>
    /// <param name="refreshToken">The refresh token, or null to keep the current one.</param>
    /// <param name="expiresAt">Instant (UTC) at which the access token expires, or null if unknown.</param>
    public void ApplyAuthorization(string? accessToken, string? refreshToken, DateTimeOffset? expiresAt)
    {
        AccessToken = accessToken;
        if (!string.IsNullOrWhiteSpace(refreshToken)) RefreshToken = refreshToken;
        TokenExpiresAt = expiresAt;
    }

    /// <summary>Records a successful sync, stamping the time and transitioning the connection to "connected".</summary>
    /// <param name="syncedAt">Instant (UTC) at which the sync completed.</param>
    public void ApplySync(DateTimeOffset syncedAt)
    {
        LastSyncedAt = syncedAt;
        Status = new WearableStatus("connected").Value;
    }

    /// <summary>Enables or disables automatic re-syncing for this connection.</summary>
    /// <param name="enabled">True to enable automatic syncing; false to disable it.</param>
    public void SetAutoSync(bool enabled)
    {
        AutoSyncEnabled = enabled;
    }

    /// <summary>Transitions the connection to the "disconnected" state, revoking further syncing and clearing tokens.</summary>
    public void ApplyDisconnect()
    {
        Status = new WearableStatus("disconnected").Value;
        AutoSyncEnabled = false;
        AccessToken = null;
        RefreshToken = null;
        TokenExpiresAt = null;
    }
}
