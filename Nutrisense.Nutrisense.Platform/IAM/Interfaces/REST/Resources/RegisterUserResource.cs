namespace Nutrisense.Nutrisense.Platform.IAM.Interfaces.REST.Resources;

public record RegisterUserResource(
    string Email,
    string Password,
    string FirstName,
    string LastName,
    string PreferredLanguage)
{
    /// <summary>Email address for the new account. Must be unique and valid email format.</summary>
    public string Email { get; init; } = Email;

    /// <summary>Password for the new account. Must meet security requirements (minimum 8 characters, uppercase, lowercase, digit, special character).</summary>
    public string Password { get; init; } = Password;

    /// <summary>User's first name.</summary>
    public string FirstName { get; init; } = FirstName;

    /// <summary>User's last name.</summary>
    public string LastName { get; init; } = LastName;

    /// <summary>Preferred language for the interface. Valid values: en, es.</summary>
    public string PreferredLanguage { get; init; } = PreferredLanguage;
}
