using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nutrisense.Nutrisense.Platform.BodyHealthMetrics.Application.Errors;
using Nutrisense.Nutrisense.Platform.BodyHealthMetrics.Application.CommandServices;
using Nutrisense.Nutrisense.Platform.BodyHealthMetrics.Application.QueryServices;
using Nutrisense.Nutrisense.Platform.BodyHealthMetrics.Domain.Model.Aggregates;
using Nutrisense.Nutrisense.Platform.BodyHealthMetrics.Domain.Model.Queries;
using Nutrisense.Nutrisense.Platform.BodyHealthMetrics.Interfaces.REST.Resources;
using Nutrisense.Nutrisense.Platform.BodyHealthMetrics.Interfaces.REST.Transform;
using Nutrisense.Nutrisense.Platform.Shared.Application.Patterns;
using Swashbuckle.AspNetCore.Annotations;
using System.Linq;

namespace Nutrisense.Nutrisense.Platform.BodyHealthMetrics.Interfaces.REST;

/// <summary>REST controller exposing body-metrics, weight, measurement, goal, and caloric-goal endpoints.</summary>
// TODO (IDOR): validate that route {userId} == authenticated user's "sub" claim before serving data.
[ApiController]
[Route("api/v1/body-metrics")]
[Authorize]
[Tags("Body Metrics")]
[Produces("application/json")]
[Consumes("application/json")]
public class BodyMetricsController(
    IBodyMetricsCommandService commandService,
    IBodyMetricsQueryService queryService) : ControllerBase
{
    [HttpPost]
    [SwaggerOperation(
        Summary = "Register body metrics for a user",
        Description = "Creates a new body metrics profile with initial biometrics. Returns the created profile with computed health values.")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register([FromBody] RegisterBodyMetricsResource resource)
    {
        try
        {
            var command = RegisterBodyMetricsCommandAssembler.ToCommand(resource);
            var result = await commandService.Handle(command);
            return result switch
            {
                Result<BodyMetrics, RegisterBodyMetricsError>.Success s =>
                    CreatedAtAction(nameof(GetByUserId), new { userId = s.Value.UserId },
                        BodyMetricsResourceAssembler.ToResource(s.Value)),
                Result<BodyMetrics, RegisterBodyMetricsError>.Failure { Error: RegisterBodyMetricsError.AlreadyExists } =>
                    Conflict("Body metrics already registered for this user."),
                Result<BodyMetrics, RegisterBodyMetricsError>.Failure { Error: RegisterBodyMetricsError.InvalidData } =>
                    BadRequest("Invalid body metrics data."),
                _ => StatusCode(StatusCodes.Status500InternalServerError)
            };
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{userId:int}/weight")]
    [SwaggerOperation(
        Summary = "Update weight for a user",
        Description = "Updates current weight and optional note. Returns updated body metrics with recalculated BMI and TDEE.")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateWeight(int userId, [FromBody] UpdateWeightResource resource)
    {
        var command = UpdateWeightCommandAssembler.ToCommand(userId, resource);
        var result = await commandService.Handle(command);
        return result switch
        {
            Result<BodyMetrics, UpdateWeightError>.Success s =>
                Ok(BodyMetricsResourceAssembler.ToResource(s.Value)),
            Result<BodyMetrics, UpdateWeightError>.Failure { Error: UpdateWeightError.BodyMetricsNotFound } =>
                NotFound(),
            Result<BodyMetrics, UpdateWeightError>.Failure { Error: UpdateWeightError.InvalidData } =>
                BadRequest("Invalid weight value."),
            _ => StatusCode(StatusCodes.Status500InternalServerError)
        };
    }

    [HttpPost("{userId:int}/body-measurements")]
    [SwaggerOperation(
        Summary = "Register a body measurement",
        Description = "Records waist and neck circumference measurements. Returns updated body metrics.")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RegisterBodyMeasurement(int userId, [FromBody] RegisterBodyMeasurementResource resource)
    {
        var command = RegisterBodyMeasurementCommandAssembler.ToCommand(userId, resource);
        var result = await commandService.Handle(command);
        return result switch
        {
            Result<BodyMetrics, RegisterBodyMeasurementError>.Success s =>
                StatusCode(StatusCodes.Status201Created, BodyMetricsResourceAssembler.ToResource(s.Value)),
            Result<BodyMetrics, RegisterBodyMeasurementError>.Failure { Error: RegisterBodyMeasurementError.BodyMetricsNotFound } =>
                NotFound(),
            Result<BodyMetrics, RegisterBodyMeasurementError>.Failure { Error: RegisterBodyMeasurementError.InvalidData } =>
                BadRequest("Invalid measurement values."),
            _ => StatusCode(StatusCodes.Status500InternalServerError)
        };
    }
}
