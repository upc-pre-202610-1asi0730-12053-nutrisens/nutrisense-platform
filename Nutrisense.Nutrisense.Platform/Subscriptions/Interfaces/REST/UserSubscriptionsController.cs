using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nutrisense.Nutrisense.Platform.Shared.Application.Patterns;
using Nutrisense.Nutrisense.Platform.Shared.Interfaces.REST.Resources;
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
    [SwaggerResponse(StatusCodes.Status201Created, "Subscription plan selected successfully. Returns the activated subscription.", typeof(UserSubscriptionResource))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authentication is required to access this resource.")]
    [SwaggerResponse(StatusCodes.Status402PaymentRequired, "The payment could not be processed.", typeof(ErrorResponse))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "The subscription plan or payment method was not found.", typeof(ErrorResponse))]
    [SwaggerResponse(StatusCodes.Status409Conflict, "The user already has an active subscription.", typeof(ErrorResponse))]
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
                NotFound(new ErrorResponse("The requested subscription plan was not found.")),
            Result<UserSubscription, SelectSubscriptionPlanError>.Failure { Error: SelectSubscriptionPlanError.PaymentMethodNotFound } =>
                NotFound(new ErrorResponse("The selected payment method was not found.")),
            Result<UserSubscription, SelectSubscriptionPlanError>.Failure { Error: SelectSubscriptionPlanError.AlreadySubscribed } =>
                Conflict(new ErrorResponse("The user already has an active subscription.")),
            Result<UserSubscription, SelectSubscriptionPlanError>.Failure { Error: SelectSubscriptionPlanError.PaymentFailed } =>
                StatusCode(StatusCodes.Status402PaymentRequired, new ErrorResponse("The payment could not be processed.")),
            _ => StatusCode(StatusCodes.Status500InternalServerError)
        };
    }

    [HttpGet("by-user/{userId:int}")]
    [SwaggerOperation(Summary = "Get user subscription", Description = "Retrieves the active subscription for a specific user if it exists.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Subscription retrieved successfully.", typeof(UserSubscriptionResource))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authentication is required to access this resource.")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "No active subscription was found for this user.", typeof(ErrorResponse))]
    public async Task<IActionResult> GetByUser(int userId)
    {
        var subscription = await queryService.Handle(new GetUserSubscriptionByUserIdQuery(userId));
        return subscription is null ? NotFound() : Ok(UserSubscriptionResourceAssembler.ToResource(subscription));
    }

    [HttpPost("{id:int}/cancel")]
    [SwaggerOperation(Summary = "Cancel subscription", Description = "Cancels an active subscription either immediately or at the end of the current billing period.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Subscription cancelled successfully. Returns the updated subscription.", typeof(UserSubscriptionResource))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "The subscription is not active and cannot be cancelled.", typeof(ErrorResponse))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authentication is required to access this resource.")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "The requested subscription was not found.", typeof(ErrorResponse))]
    public async Task<IActionResult> Cancel(int id, [FromBody] CancelSubscriptionResource resource)
    {
        var command = new CancelSubscriptionCommand(id, resource.CancelAtPeriodEnd);
        var result = await commandService.HandleCancel(command);
        return result switch
        {
            Result<UserSubscription, CancelSubscriptionError>.Success s =>
                Ok(UserSubscriptionResourceAssembler.ToResource(s.Value)),
            Result<UserSubscription, CancelSubscriptionError>.Failure { Error: CancelSubscriptionError.NotFound } =>
                NotFound(new ErrorResponse("The requested subscription was not found.")),
            Result<UserSubscription, CancelSubscriptionError>.Failure { Error: CancelSubscriptionError.NotActive } =>
                BadRequest(new ErrorResponse("The subscription is not active and cannot be cancelled.")),
            _ => StatusCode(StatusCodes.Status500InternalServerError)
        };
    }

    [HttpPost("{id:int}/renew")]
    [SwaggerOperation(Summary = "Renew subscription", Description = "Renews an expired or expiring subscription by processing a payment.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Subscription renewed successfully. Returns the updated subscription.", typeof(UserSubscriptionResource))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authentication is required to access this resource.")]
    [SwaggerResponse(StatusCodes.Status402PaymentRequired, "The payment could not be processed.", typeof(ErrorResponse))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "The requested subscription was not found.", typeof(ErrorResponse))]
    public async Task<IActionResult> Renew(int id)
    {
        var result = await commandService.HandleRenew(new RenewSubscriptionCommand(id));
        return result switch
        {
            Result<UserSubscription, RenewSubscriptionError>.Success s =>
                Ok(UserSubscriptionResourceAssembler.ToResource(s.Value)),
            Result<UserSubscription, RenewSubscriptionError>.Failure { Error: RenewSubscriptionError.NotFound } =>
                NotFound(new ErrorResponse("The requested subscription was not found.")),
            Result<UserSubscription, RenewSubscriptionError>.Failure { Error: RenewSubscriptionError.PaymentFailed } =>
                StatusCode(StatusCodes.Status402PaymentRequired, new ErrorResponse("The payment could not be processed.")),
            _ => StatusCode(StatusCodes.Status500InternalServerError)
        };
    }

    [HttpPut("{id:int}/plan")]
    [SwaggerOperation(Summary = "Change subscription plan", Description = "Upgrades or downgrades an active subscription to a different plan with optional payment adjustment.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Subscription plan changed successfully. Returns the updated subscription.", typeof(UserSubscriptionResource))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authentication is required to access this resource.")]
    [SwaggerResponse(StatusCodes.Status402PaymentRequired, "The payment could not be processed.", typeof(ErrorResponse))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "The subscription or the target plan was not found.", typeof(ErrorResponse))]
    [SwaggerResponse(StatusCodes.Status409Conflict, "The subscription is already on the requested plan.", typeof(ErrorResponse))]
    public async Task<IActionResult> ChangePlan(int id, [FromBody] ChangeSubscriptionPlanResource resource)
    {
        var command = new ChangeSubscriptionPlanCommand(id, resource.NewPlanKey, resource.BillingPeriod, resource.PaymentMethodId);
        var result = await commandService.HandleChangePlan(command);
        return result switch
        {
            Result<UserSubscription, ChangeSubscriptionPlanError>.Success s =>
                Ok(UserSubscriptionResourceAssembler.ToResource(s.Value)),
            Result<UserSubscription, ChangeSubscriptionPlanError>.Failure { Error: ChangeSubscriptionPlanError.SubscriptionNotFound } =>
                NotFound(new ErrorResponse("The requested subscription was not found.")),
            Result<UserSubscription, ChangeSubscriptionPlanError>.Failure { Error: ChangeSubscriptionPlanError.PlanNotFound } =>
                NotFound(new ErrorResponse("The target subscription plan was not found.")),
            Result<UserSubscription, ChangeSubscriptionPlanError>.Failure { Error: ChangeSubscriptionPlanError.SamePlan } =>
                Conflict(new ErrorResponse("The subscription is already on the requested plan.")),
            Result<UserSubscription, ChangeSubscriptionPlanError>.Failure { Error: ChangeSubscriptionPlanError.PaymentFailed } =>
                StatusCode(StatusCodes.Status402PaymentRequired, new ErrorResponse("The payment could not be processed.")),
            _ => StatusCode(StatusCodes.Status500InternalServerError)
        };
    }
}
