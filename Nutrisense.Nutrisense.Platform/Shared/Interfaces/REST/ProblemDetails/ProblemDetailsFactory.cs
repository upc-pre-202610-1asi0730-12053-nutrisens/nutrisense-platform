using Microsoft.AspNetCore.Http;
using AspNetProblemDetails = Microsoft.AspNetCore.Mvc.ProblemDetails;

namespace Nutrisense.Nutrisense.Platform.Shared.Interfaces.REST.ProblemDetails;

/// <summary>Builds RFC 7807 <see cref="AspNetProblemDetails"/> payloads with a status-derived Type link.</summary>
public static class ProblemDetailsFactory
{
    public static AspNetProblemDetails Create(int status, string title, string detail, string? instance = null) =>
        new()
        {
            Type = TypeForStatus(status),
            Title = title,
            Status = status,
            Detail = detail,
            Instance = instance
        };

    private static string TypeForStatus(int status) => status switch
    {
        StatusCodes.Status400BadRequest => "https://tools.ietf.org/html/rfc7231#section-6.5.1",
        StatusCodes.Status401Unauthorized => "https://tools.ietf.org/html/rfc7235#section-3.1",
        StatusCodes.Status403Forbidden => "https://tools.ietf.org/html/rfc7231#section-6.5.3",
        StatusCodes.Status404NotFound => "https://tools.ietf.org/html/rfc7231#section-6.5.4",
        StatusCodes.Status409Conflict => "https://tools.ietf.org/html/rfc7231#section-6.5.8",
        StatusCodes.Status422UnprocessableEntity => "https://tools.ietf.org/html/rfc4918#section-11.2",
        StatusCodes.Status500InternalServerError => "https://tools.ietf.org/html/rfc7231#section-6.6.1",
        _ => "about:blank"
    };
}
