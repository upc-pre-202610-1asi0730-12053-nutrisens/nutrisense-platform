namespace Nutrisense.Nutrisense.Platform.Shared.Interfaces.REST.Resources;

/// <summary>Standard error payload returned for failed requests, with a human-readable message.</summary>
public record ErrorResponse(string Message)
{
    /// <summary>Human-readable description of what went wrong, localized to the request culture.</summary>
    public string Message { get; init; } = Message;
}
