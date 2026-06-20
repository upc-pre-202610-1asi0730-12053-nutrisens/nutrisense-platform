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
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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
                BadRequest("Invalid card details."),
            _ => StatusCode(StatusCodes.Status500InternalServerError)
        };
    }

    [HttpGet("by-user/{userId:int}")]
    [SwaggerOperation(Summary = "Get user payment methods", Description = "Retrieves all payment methods registered for a specific user.")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetByUser(int userId)
    {
        var methods = await queryService.Handle(new GetPaymentMethodsByUserIdQuery(userId));
        return Ok(methods.Select(PaymentMethodResourceAssembler.ToResource));
    }

    [HttpDelete("{id:int}")]
    [SwaggerOperation(Summary = "Delete payment method", Description = "Removes a payment method from a user's account. Returns 204 on success.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Remove(int id)
    {
        var removed = await commandService.RemoveAsync(id);
        return removed ? NoContent() : NotFound();
    }
}
