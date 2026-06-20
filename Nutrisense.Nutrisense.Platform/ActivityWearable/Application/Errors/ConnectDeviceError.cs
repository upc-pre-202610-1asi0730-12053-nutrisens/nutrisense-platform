namespace Nutrisense.Nutrisense.Platform.ActivityWearable.Application.Errors;

/// <summary>Failure modes for connecting a wearable device.</summary>
public enum ConnectDeviceError
{
    /// <summary>The user already has an active connection for the requested provider.</summary>
    AlreadyConnected,

    /// <summary>The provider rejected the supplied OAuth authorization code.</summary>
    AuthorizationFailed,

    /// <summary>The requested provider is not supported.</summary>
    InvalidProvider,

    /// <summary>An unforeseen error occurred while connecting the device.</summary>
    UnexpectedError
}
