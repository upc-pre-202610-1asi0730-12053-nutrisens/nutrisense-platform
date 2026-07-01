using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Nutrisense.Nutrisense.Platform.BodyHealthMetrics.Domain.Model.Errors;
using Nutrisense.Nutrisense.Platform.BodyHealthMetrics.Resources;
using Nutrisense.Nutrisense.Platform.Shared.Application.Patterns;
using ProblemDetailsFactory = Nutrisense.Nutrisense.Platform.Shared.Interfaces.REST.ProblemDetails.ProblemDetailsFactory;

namespace Nutrisense.Nutrisense.Platform.BodyHealthMetrics.Interfaces.REST.Transform;

/// <summary>Maps any BodyHealthMetrics failure to a localized RFC 7807 problem-details response.</summary>
public static class BodyHealthMetricsActionResultAssembler
{
    public static IActionResult ToActionResult(
        BodyHealthMetricsError error,
        IStringLocalizer<BodyHealthMetricsMessages> localizer,
        string? instance = null)
    {
        var status = StatusFor(error);
        var titleKey = $"{error}Title";
        var detail = localizer[error.ToString()].Value;
        var title = localizer[status == StatusCodes.Status500InternalServerError ? "UnexpectedServerError" : titleKey].Value;

        return new ObjectResult(ProblemDetailsFactory.Create(status, title, detail, instance)) { StatusCode = status };
    }

    private static int StatusFor(BodyHealthMetricsError error) => error switch
    {
        BodyHealthMetricsError.BodyMetricsNotFound => StatusCodes.Status404NotFound,
        BodyHealthMetricsError.NoActiveGoal => StatusCodes.Status404NotFound,
        BodyHealthMetricsError.TdeeNotCalculated => StatusCodes.Status404NotFound,
        BodyHealthMetricsError.BodyMetricsAlreadyExists => StatusCodes.Status409Conflict,
        BodyHealthMetricsError.InvalidBodyMetricsData => StatusCodes.Status400BadRequest,
        BodyHealthMetricsError.InvalidWeightValue => StatusCodes.Status400BadRequest,
        BodyHealthMetricsError.InvalidMeasurementValues => StatusCodes.Status400BadRequest,
        BodyHealthMetricsError.InvalidGoalData => StatusCodes.Status400BadRequest,
        _ => StatusCodes.Status500InternalServerError
    };
}
