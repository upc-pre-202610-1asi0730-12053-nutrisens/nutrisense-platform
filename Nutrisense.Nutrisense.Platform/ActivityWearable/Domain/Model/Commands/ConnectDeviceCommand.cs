namespace Nutrisense.Nutrisense.Platform.ActivityWearable.Domain.Model.Commands;

/// <summary>Represents the intention to connect a wearable device for a user.</summary>
/// <param name="UserId">Identifier of the user connecting the device.</param>
/// <param name="Provider">External provider to connect to (e.g. "google-fit").</param>
/// <param name="OAuthCode">OAuth authorization code obtained from the provider's consent flow.</param>
public record ConnectDeviceCommand(int UserId, string Provider, string OAuthCode);
