namespace Nutrisense.Nutrisense.Platform.ActivityWearable.Interfaces.REST.Resources;

/// <summary>Output resource representing a wearable connection returned to API clients.</summary>
/// <param name="Id">Unique identifier of the wearable connection.</param>
/// <param name="UserId">Identifier of the user the connection belongs to.</param>
/// <param name="Provider">External provider the connection authorizes against (e.g. "google-fit").</param>
/// <param name="Status">Lifecycle state: "pending-auth", "connected" or "disconnected".</param>
/// <param name="LastSyncedAt">Instant (UTC) of the last successful sync, or null if never synced.</param>
/// <param name="AuthorizedAt">Instant (UTC) at which the user authorized the connection.</param>
public record WearableConnectionResource(
    int Id,
    int UserId,
    string Provider,
    string Status,
    DateTimeOffset? LastSyncedAt,
    DateTimeOffset AuthorizedAt);
