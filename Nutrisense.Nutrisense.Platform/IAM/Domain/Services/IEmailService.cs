namespace Nutrisense.Nutrisense.Platform.IAM.Domain.Services;

/// <summary>
/// Domain service that abstracts the sending of transactional emails.
/// Concrete implementations live in the infrastructure layer.
/// </summary>
public interface IEmailService
{
    /// <summary>
    /// Sends a password reset email containing the recovery link.
    /// </summary>
    /// <param name="toEmail">Recipient email address.</param>
    /// <param name="resetToken">Token embedded in the reset link.</param>
    Task SendPasswordResetEmailAsync(string toEmail, string resetToken, CancellationToken ct = default);
}
