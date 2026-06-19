namespace Nutrisense.Nutrisense.Platform.IAM.Interfaces.REST.Resources;

public record LoginUserResource(string Email, string Password, string? DeviceLabel)
{
    /// <summary>Email address of the user account.</summary>
    public string Email { get; init; } = Email;

    /// <summary>Password for the user account.</summary>
    public string Password { get; init; } = Password;

    /// <summary>Optional label for the device/session (e.g., "iPhone 14", "Desktop"). If not provided, a default label is generated.</summary>
    public string? DeviceLabel { get; init; } = DeviceLabel;
}
