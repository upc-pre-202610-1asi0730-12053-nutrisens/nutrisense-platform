namespace Nutrisense.Nutrisense.Platform.ActivityWearable.Interfaces.REST.Resources;

/// <summary>Input resource for connecting a wearable device.</summary>
/// <param name="UserId">Identifier of the user connecting the device.</param>
/// <param name="Provider">External provider to connect to (e.g. "google-fit").</param>
/// <param name="OAuthCode">OAuth authorization code obtained from the provider's consent flow.</param>
public record ConnectDeviceResource(int UserId, string Provider, string OAuthCode);
