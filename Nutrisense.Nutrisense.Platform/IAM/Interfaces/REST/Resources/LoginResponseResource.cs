namespace Nutrisense.Nutrisense.Platform.IAM.Interfaces.REST.Resources;

public record LoginResponseResource(int UserId, string Token, int SessionId)
{
    /// <summary>Unique identifier of the authenticated user.</summary>
    public int UserId { get; init; } = UserId;

    /// <summary>JWT authentication token for subsequent API requests. Include as Bearer token in Authorization header.</summary>
    public string Token { get; init; } = Token;

    /// <summary>Unique identifier for this session. Used to manage or revoke the session later.</summary>
    public int SessionId { get; init; } = SessionId;
}
