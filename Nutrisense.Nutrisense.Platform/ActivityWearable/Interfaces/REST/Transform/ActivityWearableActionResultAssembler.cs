using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Nutrisense.Nutrisense.Platform.ActivityWearable.Domain.Model.Aggregates;
using Nutrisense.Nutrisense.Platform.ActivityWearable.Domain.Model.Errors;
using Nutrisense.Nutrisense.Platform.ActivityWearable.Resources;
using Nutrisense.Nutrisense.Platform.Shared.Application.Patterns;
using ProblemDetailsFactory = Nutrisense.Nutrisense.Platform.Shared.Interfaces.REST.ProblemDetails.ProblemDetailsFactory;

namespace Nutrisense.Nutrisense.Platform.ActivityWearable.Interfaces.REST.Transform;

/// <summary>Centralized enum-to-HTTP mapping for every <see cref="ActivityWearableError"/> failure across the context.</summary>
public static class ActivityWearableActionResultAssembler
{
    public static IActionResult ToActionResult<TValue>(
        Result<TValue, ActivityWearableError> result,
        IStringLocalizer<ActivityWearableMessages> localizer,
        Func<TValue, IActionResult> onSuccess,
        string? instance = null)
    {
        if (result is Result<TValue, ActivityWearableError>.Success s)
            return onSuccess(s.Value);

        var error = ((Result<TValue, ActivityWearableError>.Failure)result).Error;
        var (status, detailKey) = error switch
        {
            ActivityWearableError.AlreadyConnected => (StatusCodes.Status409Conflict, "DeviceAlreadyConnected"),
            ActivityWearableError.AuthorizationFailed => (StatusCodes.Status400BadRequest, "OAuthAuthorizationFailed"),
            ActivityWearableError.InvalidProvider => (StatusCodes.Status400BadRequest, "InvalidWearableProvider"),
            ActivityWearableError.ActivityLogNotFound => (StatusCodes.Status404NotFound, "ActivityLogNotFound"),
            ActivityWearableError.ActivityLogNotOwner => (StatusCodes.Status403Forbidden, "ActivityLogDeleteNotOwner"),
            ActivityWearableError.WearableConnectionNotFound => (StatusCodes.Status404NotFound, "WearableConnectionNotFound"),
            ActivityWearableError.InvalidActivity => (StatusCodes.Status400BadRequest, "InvalidActivityData"),
            ActivityWearableError.SyncFailed => (StatusCodes.Status400BadRequest, "ActivitySyncFailed"),
            _ => (StatusCodes.Status500InternalServerError, "UnexpectedError")
        };

        var titleKey = status switch
        {
            StatusCodes.Status404NotFound => "NotFoundTitle",
            StatusCodes.Status403Forbidden => "Forbidden",
            StatusCodes.Status409Conflict => "ConflictTitle",
            StatusCodes.Status400BadRequest => "BadRequestTitle",
            _ => "UnexpectedServerError"
        };

        var problemDetails = ProblemDetailsFactory.Create(status, localizer[titleKey].Value, localizer[detailKey].Value, instance);
        return new ObjectResult(problemDetails) { StatusCode = status };
    }
}
