namespace Nutrisense.Nutrisense.Platform.ActivityWearable.Domain.Model.Errors;

/// <summary>Unified failure modes for the ActivityWearable bounded context.</summary>
public enum ActivityWearableError
{
    /// <summary>The user already has an active connection for the requested provider.</summary>
    AlreadyConnected,

    /// <summary>The provider rejected the supplied OAuth authorization code.</summary>
    AuthorizationFailed,

    /// <summary>The requested provider is not supported.</summary>
    InvalidProvider,

    /// <summary>No activity log exists with the supplied identifier.</summary>
    ActivityLogNotFound,

    /// <summary>The requesting user is not the owner of the activity log.</summary>
    ActivityLogNotOwner,

    /// <summary>No wearable connection exists with the supplied identifier.</summary>
    WearableConnectionNotFound,

    /// <summary>The submitted activity data failed domain validation.</summary>
    InvalidActivity,

    /// <summary>The provider failed to return activity data.</summary>
    SyncFailed,

    /// <summary>An unforeseen error occurred.</summary>
    UnexpectedError
}
