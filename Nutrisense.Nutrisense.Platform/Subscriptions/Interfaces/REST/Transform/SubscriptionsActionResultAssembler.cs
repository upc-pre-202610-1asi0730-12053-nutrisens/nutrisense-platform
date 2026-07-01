using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Nutrisense.Nutrisense.Platform.Shared.Application.Patterns;
using Nutrisense.Nutrisense.Platform.Subscriptions.Domain.Model.Errors;
using Nutrisense.Nutrisense.Platform.Subscriptions.Resources;
using ProblemDetailsFactory = Nutrisense.Nutrisense.Platform.Shared.Interfaces.REST.ProblemDetails.ProblemDetailsFactory;

namespace Nutrisense.Nutrisense.Platform.Subscriptions.Interfaces.REST.Transform;

/// <summary>Converts a <see cref="SubscriptionsError"/> failure into an RFC 7807 problem details result.</summary>
public static class SubscriptionsActionResultAssembler
{
    public static IActionResult ToActionResult<TValue>(
        Result<TValue, SubscriptionsError>.Failure failure,
        IStringLocalizer<SubscriptionsMessages> localizer,
        string? instance = null)
    {
        var status = StatusFor(failure.Error);
        var titleKey = failure.Error == SubscriptionsError.UnexpectedError
            ? "UnexpectedServerError"
            : $"{failure.Error}Title";
        var detailKey = failure.Error.ToString();

        return new ObjectResult(ProblemDetailsFactory.Create(
            status, localizer[titleKey].Value, localizer[detailKey].Value, instance))
        {
            StatusCode = status
        };
    }

    private static int StatusFor(SubscriptionsError error) => error switch
    {
        SubscriptionsError.SubscriptionNotFound => StatusCodes.Status404NotFound,
        SubscriptionsError.PlanNotFound => StatusCodes.Status404NotFound,
        SubscriptionsError.PaymentMethodNotFound => StatusCodes.Status404NotFound,
        SubscriptionsError.SubscriptionNotActive => StatusCodes.Status400BadRequest,
        SubscriptionsError.InvalidCard => StatusCodes.Status400BadRequest,
        SubscriptionsError.SamePlan => StatusCodes.Status409Conflict,
        SubscriptionsError.AlreadySubscribed => StatusCodes.Status409Conflict,
        SubscriptionsError.PaymentFailed => StatusCodes.Status402PaymentRequired,
        _ => StatusCodes.Status500InternalServerError
    };
}
