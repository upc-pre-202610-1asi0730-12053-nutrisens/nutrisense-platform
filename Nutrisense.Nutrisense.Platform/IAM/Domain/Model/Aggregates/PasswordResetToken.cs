namespace Nutrisense.Nutrisense.Platform.IAM.Domain.Model.Aggregates;

/// <summary>
/// Domain aggregate representing a single-use password reset token.
/// <para>
/// Each token is valid for one hour from creation and is invalidated once used.
/// A user should only hold one active token at a time; previous tokens are removed
/// when a new reset is requested (see <c>IPasswordResetTokenRepository.DeleteByUserIdAsync</c>).
/// </para>
/// </summary>
public class PasswordResetToken
{
    public int Id { get; private set; }
    public string Token { get; private set; } = null!;
    public int UserId { get; private set; }
    public DateTimeOffset ExpiresAt { get; private set; }
    public bool Used { get; private set; }

    // Required by EF Core.
    protected PasswordResetToken() { }

    private PasswordResetToken(int userId, string token, DateTimeOffset expiresAt)
    {
        UserId = userId;
        Token = token;
        ExpiresAt = expiresAt;
        Used = false;
    }

    /// <summary>
    /// Creates a new reset token (random GUID) valid for one hour.
    /// </summary>
    /// <param name="userId">Id of the user requesting the reset.</param>
    public static PasswordResetToken Create(int userId) =>
        new(userId, Guid.NewGuid().ToString("N"), DateTimeOffset.UtcNow.AddHours(1));

    public bool IsExpired() => DateTimeOffset.UtcNow > ExpiresAt;

    public bool IsValid() => !Used && !IsExpired();

    public void MarkAsUsed() => Used = true;
}
