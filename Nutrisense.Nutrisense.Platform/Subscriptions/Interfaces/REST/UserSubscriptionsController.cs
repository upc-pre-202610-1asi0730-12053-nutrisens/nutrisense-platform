using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nutrisense.Nutrisense.Platform.Shared.Application.Patterns;
using Nutrisense.Nutrisense.Platform.Subscriptions.Application.Errors;
using Nutrisense.Nutrisense.Platform.Subscriptions.Application.CommandServices;
using Nutrisense.Nutrisense.Platform.Subscriptions.Application.QueryServices;
using Nutrisense.Nutrisense.Platform.Subscriptions.Domain.Model.Aggregates;
using Nutrisense.Nutrisense.Platform.Subscriptions.Domain.Model.Commands;
using Nutrisense.Nutrisense.Platform.Subscriptions.Domain.Model.Queries;
using Nutrisense.Nutrisense.Platform.Subscriptions.Interfaces.REST.Resources;
using Nutrisense.Nutrisense.Platform.Subscriptions.Interfaces.REST.Transform;
using Swashbuckle.AspNetCore.Annotations;

namespace Nutrisense.Nutrisense.Platform.Subscriptions.Interfaces.REST;

// TODO (IDOR): validate that route {userId} == authenticated user's "sub" claim before serving data.
[ApiController]
[Route("api/v1/user-subscriptions")]
[Authorize]
[Tags("User Subscriptions")]
[Produces("application/json")]
[Consumes("application/json")]
public class UserSubscriptionsController(
    IUserSubscriptionCommandService commandService,
    IUserSubscriptionQueryService queryService) : ControllerBase
{
    [HttpPost]
    [SwaggerOperation(Summary = "Select subscription plan", Description = "Selects a subscription plan for a user and initiates the activation chain including payment processing.")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status402PaymentRequired)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> SelectPlan([FromBody] SelectSubscriptionPlanResource resource)
    {
        var command = new SelectSubscriptionPlanCommand(resource.UserId, resource.PlanKey, resource.PaymentMethodId, resource.BillingPeriod);
        var result = await commandService.HandleSelectPlan(command);
        return result switch
        {
            Result<UserSubscription, SelectSubscriptionPlanError>.Success s =>
                CreatedAtAction(nameof(GetByUser), new { userId = s.Value.UserId },
                    UserSubscriptionResourceAssembler.ToResource(s.Value)),
            Result<UserSubscription, SelectSubscriptionPlanError>.Failure { Error: SelectSubscriptionPlanError.PlanNotFound } =>
                NotFound("Subscription plan not found."),
            Result<UserSubscription, SelectSubscriptionPlanError>.Failure { Error: SelectSubscriptionPlanError.PaymentMethodNotFound } =>
                NotFound("Payment method not found."),
            Result<UserSubscription, SelectSubscriptionPlanError>.Failure { Error: SelectSubscriptionPlanError.AlreadySubscribed } =>
                Conflict("User already has an active subscription."),
            Result<UserSubscription, SelectSubscriptionPlanError>.Failure { Error: SelectSubscriptionPlanError.PaymentFailed } =>
                StatusCode(StatusCodes.Status402PaymentRequired, "Payment failed."),
            _ => StatusCode(StatusCodes.Status500InternalServerError)
        };
    }

    [HttpGet("by-user/{userId:int}")]
    [SwaggerOperation(Summary = "Get user subscription", Description = "Retrieves the active subscription for a specific user if it exists.")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetByUser(int userId)
    {
        var subscription = await queryService.Handle(new GetUserSubscriptionByUserIdQuery(userId));
        return subscription is null ? NotFound() : Ok(UserSubscriptionResourceAssembler.ToResource(subscription));
    }

    [HttpPost("{id:int}/cancel")]
    [SwaggerOperation(Summary = "Cancel subscription", Description = "Cancels an active subscription either immediately or at the end of the current billing period.")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Cancel(int id, [FromBody] CancelSubscriptionResource resource)
    {
        var command = new CancelSubscriptionCommand(id, resource.CancelAtPeriodEnd);
        var result = await commandService.HandleCancel(command);
        return result switch
        {
            Result<UserSubscription, CancelSubscriptionError>.Success s =>
                Ok(UserSubscriptionResourceAssembler.ToResource(s.Value)),
            Result<UserSubscription, CancelSubscriptionError>.Failure { Error: CancelSubscriptionError.NotFound } =>
                NotFound(),
            Result<UserSubscription, CancelSubscriptionError>.Failure { Error: CancelSubscriptionError.NotActive } =>
                BadRequest("Subscription is not active."),
            _ => StatusCode(StatusCodes.Status500InternalServerError)
        };
    }

    [HttpPost("{id:int}/renew")]
    [SwaggerOperation(Summary = "Renew subscription", Description = "Renews an expired or expiring subscription by processing a payment.")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status402PaymentRequired)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Renew(int id)
    {
        var result = await commandService.HandleRenew(new RenewSubscriptionCommand(id));
        return result switch
        {
            Result<UserSubscription, RenewSubscriptionError>.Success s =>
                Ok(UserSubscriptionResourceAssembler.ToResource(s.Value)),
            Result<UserSubscription, RenewSubscriptionError>.Failure { Error: RenewSubscriptionError.NotFound } =>
                NotFound(),
            Result<UserSubscription, RenewSubscriptionError>.Failure { Error: RenewSubscriptionError.PaymentFailed } =>
                StatusCode(StatusCodes.Status402PaymentRequired, "Payment failed."),
            _ => StatusCode(StatusCodes.Status500InternalServerError)
        };
    }
}
