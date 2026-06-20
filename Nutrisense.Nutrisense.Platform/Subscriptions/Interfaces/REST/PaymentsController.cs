using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nutrisense.Nutrisense.Platform.Subscriptions.Application.QueryServices;
using Nutrisense.Nutrisense.Platform.Subscriptions.Domain.Model.Queries;
using Nutrisense.Nutrisense.Platform.Subscriptions.Interfaces.REST.Transform;
using Swashbuckle.AspNetCore.Annotations;

namespace Nutrisense.Nutrisense.Platform.Subscriptions.Interfaces.REST;

[ApiController]
[Route("api/v1/payments")]
[Authorize]
[Tags("Payments")]
[Produces("application/json")]
public class PaymentsController(IUserSubscriptionQueryService queryService) : ControllerBase
{
    [HttpGet("by-subscription/{subscriptionId:int}")]
    [SwaggerOperation(Summary = "Get payment history", Description = "Retrieves payment records for a specific subscription with their status and amounts.")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetBySubscription(int subscriptionId)
    {
        var records = await queryService.Handle(new GetPaymentHistoryBySubscriptionIdQuery(subscriptionId));
        return Ok(records.Select(PaymentRecordResourceAssembler.ToResource));
    }
}
