namespace Nutrisense.Nutrisense.Platform.IAM.Interfaces.REST.Resources;

public record ForgotPasswordResource(string Email)
{
    /// <summary>Email address of the account to recover.</summary>
    public string Email { get; init; } = Email;
}
