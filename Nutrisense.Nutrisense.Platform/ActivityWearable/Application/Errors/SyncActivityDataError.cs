namespace Nutrisense.Nutrisense.Platform.ActivityWearable.Application.Errors;

/// <summary>Failure modes for syncing activity data from a wearable provider.</summary>
public enum SyncActivityDataError
{
    /// <summary>No wearable connection exists with the supplied identifier.</summary>
    ConnectionNotFound,

    /// <summary>The provider failed to return activity data.</summary>
    SyncFailed,

    /// <summary>An unforeseen error occurred while syncing activity data.</summary>
    UnexpectedError
}
