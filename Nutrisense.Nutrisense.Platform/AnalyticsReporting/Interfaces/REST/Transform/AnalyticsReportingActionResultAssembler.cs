using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Nutrisense.Nutrisense.Platform.AnalyticsReporting.Domain.Model.Errors;
using Nutrisense.Nutrisense.Platform.AnalyticsReporting.Resources;
using Nutrisense.Nutrisense.Platform.Shared.Application.Patterns;
using ProblemDetailsFactory = Nutrisense.Nutrisense.Platform.Shared.Interfaces.REST.ProblemDetails.ProblemDetailsFactory;

namespace Nutrisense.Nutrisense.Platform.AnalyticsReporting.Interfaces.REST.Transform;

/// <summary>Maps an AnalyticsReporting <see cref="Result{TValue,TError}"/> failure to the matching problem-details response.</summary>
public static class AnalyticsReportingActionResultAssembler
{
    public static ObjectResult ToActionResult(
        AnalyticsReportingError error, IStringLocalizer<AnalyticsReportingMessages> localizer, string? instance = null) =>
        error switch
        {
            _ => new ObjectResult(ProblemDetailsFactory.Create(
                StatusCodes.Status500InternalServerError,
                localizer["UnexpectedServerError"].Value,
                localizer["UnexpectedErrorProcessingRequest"].Value,
                instance))
            {
                StatusCode = StatusCodes.Status500InternalServerError
            }
        };
}
