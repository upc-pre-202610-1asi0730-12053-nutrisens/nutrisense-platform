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
[Route("api/v1/payment-methods")]
[Authorize]
[Tags("Payment Methods")]
[Produces("application/json")]
[Consumes("application/json")]
public class PaymentMethodsController(
    IPaymentMethodCommandService commandService,
    IPaymentMethodQueryService queryService) : ControllerBase
{
    [HttpPost]
    [SwaggerOperation(Summary = "Register payment method", Description = "Registers a new payment method for a user. Validates card details via Stripe.")]
    [SwaggerResponse(StatusCodes.Status201Created, "Payment method registered successfully.", typeof(PaymentMethodResource))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "The provided card details are not valid.", typeof(ErrorResponse))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authentication is required to access this resource.")]
    public async Task<IActionResult> Register([FromBody] RegisterPaymentMethodResource resource)
    {
        var command = new RegisterPaymentMethodCommand(
            resource.UserId, resource.LastFour, resource.Brand,
            resource.ExpiryMonth, resource.ExpiryYear,
            resource.CardholderName, resource.StripePaymentMethodId);

        var result = await commandService.Handle(command);
        return result switch
        {
            Result<PaymentMethod, RegisterPaymentMethodError>.Success s =>
                StatusCode(StatusCodes.Status201Created, PaymentMethodResourceAssembler.ToResource(s.Value)),
            Result<PaymentMethod, RegisterPaymentMethodError>.Failure { Error: RegisterPaymentMethodError.InvalidCard } =>
                BadRequest(new ErrorResponse("The provided card details are not valid.")),
            _ => StatusCode(StatusCodes.Status500InternalServerError)
        };
    }

    [HttpGet("by-user/{userId:int}")]
    [SwaggerOperation(Summary = "Get user payment methods", Description = "Retrieves all payment methods registered for a specific user.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Payment methods retrieved successfully.", typeof(PaymentMethodResource[]))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authentication is required to access this resource.")]
    public async Task<IActionResult> GetByUser(int userId)
    {
        var methods = await queryService.Handle(new GetPaymentMethodsByUserIdQuery(userId));
        return Ok(methods.Select(PaymentMethodResourceAssembler.ToResource));
    }

    [HttpDelete("{id:int}")]
    [SwaggerOperation(Summary = "Delete payment method", Description = "Removes a payment method from a user's account. Returns 204 on success.")]
    [SwaggerResponse(StatusCodes.Status204NoContent, "Payment method deleted successfully.")]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authentication is required to access this resource.")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "The requested payment method was not found.")]
    public async Task<IActionResult> Remove(int id)
    {
        var removed = await commandService.RemoveAsync(id);
        return removed ? NoContent() : NotFound();
    }
}
