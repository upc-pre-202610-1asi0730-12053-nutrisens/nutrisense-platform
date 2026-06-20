namespace Nutrisense.Nutrisense.Platform.ActivityWearable.Application.Errors;

/// <summary>Failure modes for disconnecting a wearable device.</summary>
public enum DisconnectDeviceError
{
    /// <summary>No wearable connection exists with the supplied identifier.</summary>
    ConnectionNotFound,

    /// <summary>An unforeseen error occurred while disconnecting the device.</summary>
    UnexpectedError
}
