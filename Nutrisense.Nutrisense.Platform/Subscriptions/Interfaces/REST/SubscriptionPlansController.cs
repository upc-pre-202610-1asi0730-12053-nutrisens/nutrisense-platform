using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nutrisense.Nutrisense.Platform.Subscriptions.Application.QueryServices;
using Nutrisense.Nutrisense.Platform.Subscriptions.Domain.Model.Queries;
using Nutrisense.Nutrisense.Platform.Subscriptions.Interfaces.REST.Resources;
using Nutrisense.Nutrisense.Platform.Subscriptions.Interfaces.REST.Transform;
using Swashbuckle.AspNetCore.Annotations;

namespace Nutrisense.Nutrisense.Platform.Subscriptions.Interfaces.REST;

[ApiController]
[Route("api/v1/subscription-plans")]
[AllowAnonymous]
[Tags("Subscription Plans")]
[Produces("application/json")]
public class SubscriptionPlansController(ISubscriptionPlanQueryService queryService) : ControllerBase
{
    [HttpGet]
    [SwaggerOperation(Summary = "Get all subscription plans", Description = "Returns a list of all available subscription plans with pricing and features.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Subscription plans retrieved successfully.", typeof(SubscriptionPlanResource[]))]
    public async Task<IActionResult> GetAll()
    {
        var plans = await queryService.Handle(new GetAllSubscriptionPlansQuery());
        return Ok(plans.Select(SubscriptionPlanResourceAssembler.ToResource));
    }

    [HttpGet("{key}")]
    [SwaggerOperation(Summary = "Get plan by key", Description = "Retrieves a specific subscription plan by its unique key identifier.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Subscription plan retrieved successfully.", typeof(SubscriptionPlanResource))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "No subscription plan was found with the specified key.")]
    public async Task<IActionResult> GetByKey(string key)
    {
        var plan = await queryService.Handle(new GetSubscriptionPlanByKeyQuery(key));
        return plan is null ? NotFound() : Ok(SubscriptionPlanResourceAssembler.ToResource(plan));
    }
}
