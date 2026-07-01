using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Nutrisense.Nutrisense.Platform.IAM.Domain.Services;

namespace Nutrisense.Nutrisense.Platform.IAM.Infrastructure.Mailing;

/// <summary>
/// Infrastructure adapter implementing <see cref="IEmailService"/> over SMTP using MailKit.
/// Builds an HTML email containing the password reset link and sends it as a MIME message.
/// SMTP credentials and the frontend base URL are read from configuration:
/// <list type="bullet">
///   <item><c>Smtp:Host</c>, <c>Smtp:Port</c>, <c>Smtp:Username</c>, <c>Smtp:Password</c>, <c>Smtp:From</c>, <c>Smtp:UseSsl</c></item>
///   <item><c>App:BaseUrl</c> — frontend origin used to build the reset link (default http://localhost:5173)</item>
/// </list>
/// </summary>
public class SmtpEmailService(IConfiguration configuration, ILogger<SmtpEmailService> logger) : IEmailService
{
    public async Task SendPasswordResetEmailAsync(string toEmail, string resetToken, CancellationToken ct = default)
    {
        var smtp = configuration.GetSection("Smtp");
        var host = smtp["Host"];
        var from = smtp["From"];

        if (string.IsNullOrWhiteSpace(host) || string.IsNullOrWhiteSpace(from))
        {
            // SMTP not configured (e.g. local dev) — log the link instead of failing the flow.
            var fallbackUrl = BuildResetUrl(resetToken);
            logger.LogWarning("SMTP is not configured. Password reset link for {Email}: {Url}", toEmail, fallbackUrl);
            return;
        }

        var port = int.TryParse(smtp["Port"], out var p) ? p : 587;
        var useSsl = bool.TryParse(smtp["UseSsl"], out var s) && s;
        var username = smtp["Username"];
        var password = smtp["Password"];

        var resetUrl = BuildResetUrl(resetToken);
        var html = $"""
            <div style="font-family:sans-serif;max-width:480px;margin:auto">
              <h2>Reset your NutriSense password</h2>
              <p>Click the button below to set a new password. The link expires in <strong>1 hour</strong>.</p>
              <a href="{resetUrl}"
                 style="display:inline-block;padding:12px 24px;background:#22c55e;color:#fff;
                        border-radius:8px;text-decoration:none;font-weight:600">
                Reset password
              </a>
              <p style="margin-top:24px;color:#6b7280;font-size:13px">
                If you didn't request a password reset, you can safely ignore this email.
              </p>
            </div>
            """;

        var message = new MimeMessage();
        message.From.Add(MailboxAddress.Parse(from));
        message.To.Add(MailboxAddress.Parse(toEmail));
        message.Subject = "Reset your NutriSense password";
        message.Body = new BodyBuilder { HtmlBody = html }.ToMessageBody();

        using var client = new SmtpClient();
        var socketOptions = useSsl ? SecureSocketOptions.SslOnConnect : SecureSocketOptions.StartTlsWhenAvailable;
        await client.ConnectAsync(host, port, socketOptions, ct);
        if (!string.IsNullOrWhiteSpace(username))
            await client.AuthenticateAsync(username, password ?? string.Empty, ct);
        await client.SendAsync(message, ct);
        await client.DisconnectAsync(true, ct);
    }

    private string BuildResetUrl(string resetToken)
    {
        var baseUrl = configuration["App:BaseUrl"] ?? "http://localhost:5173";
        return $"{baseUrl.TrimEnd('/')}/reset-password?token={resetToken}";
    }
}
