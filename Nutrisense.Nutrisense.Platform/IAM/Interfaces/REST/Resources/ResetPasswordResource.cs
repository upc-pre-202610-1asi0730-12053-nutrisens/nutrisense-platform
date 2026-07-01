namespace Nutrisense.Nutrisense.Platform.IAM.Interfaces.REST.Resources;

public record ResetPasswordResource(string Token, string NewPassword)
{
    /// <summary>The reset token received by email.</summary>
    public string Token { get; init; } = Token;

    /// <summary>New password. Must meet security requirements (min 8 chars, at least one letter and one digit).</summary>
    public string NewPassword { get; init; } = NewPassword;
}
