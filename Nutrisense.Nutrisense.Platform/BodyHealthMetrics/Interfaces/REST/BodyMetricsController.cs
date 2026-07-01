using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Nutrisense.Nutrisense.Platform.BodyHealthMetrics.Application.CommandServices;
using Nutrisense.Nutrisense.Platform.BodyHealthMetrics.Application.QueryServices;
using Nutrisense.Nutrisense.Platform.BodyHealthMetrics.Domain.Model.Aggregates;
using Nutrisense.Nutrisense.Platform.BodyHealthMetrics.Domain.Model.Errors;
using Nutrisense.Nutrisense.Platform.BodyHealthMetrics.Domain.Model.Queries;
using Nutrisense.Nutrisense.Platform.BodyHealthMetrics.Interfaces.REST.Resources;
using Nutrisense.Nutrisense.Platform.BodyHealthMetrics.Interfaces.REST.Transform;
using Nutrisense.Nutrisense.Platform.BodyHealthMetrics.Resources;
using Nutrisense.Nutrisense.Platform.Shared.Application.Patterns;
using Nutrisense.Nutrisense.Platform.Shared.Interfaces.REST.Extensions;
using Swashbuckle.AspNetCore.Annotations;
using System.Globalization;
using System.Linq;
using SharedProblemDetailsFactory = Nutrisense.Nutrisense.Platform.Shared.Interfaces.REST.ProblemDetails.ProblemDetailsFactory;

namespace Nutrisense.Nutrisense.Platform.BodyHealthMetrics.Interfaces.REST;

/// <summary>REST controller exposing body-metrics, weight, measurement, goal, and caloric-goal endpoints.</summary>
[ApiController]
[Route("api/v1/body-metrics")]
[Authorize]
[Tags("Body Metrics")]
[Produces("application/json")]
[Consumes("application/json")]
public class BodyMetricsController(
    IBodyMetricsCommandService commandService,
    IBodyMetricsQueryService queryService,
    IStringLocalizer<BodyHealthMetricsMessages> localizer) : ControllerBase
{
    [HttpPost]
    [SwaggerOperation(
        Summary = "Register body metrics for a user",
        Description = "Creates a new body metrics profile with initial biometrics. Returns the created profile with computed health values.")]
    [SwaggerResponse(StatusCodes.Status201Created, "Body metrics registered successfully. Returns the created profile with computed health values.", typeof(BodyMetricsResource))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "The provided body metrics data is not valid.", typeof(ProblemDetails))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authentication is required to access this resource.")]
    [SwaggerResponse(StatusCodes.Status409Conflict, "Body metrics are already registered for this user.", typeof(ProblemDetails))]
    public async Task<IActionResult> Register([FromBody] RegisterBodyMetricsResource resource)
    {
        if (resource.UserId != this.GetAuthenticatedUserId()) return Forbid();
        if (!DateOnly.TryParseExact(resource.DateOfBirth, "yyyy-MM-dd",
                CultureInfo.InvariantCulture, DateTimeStyles.None, out var dateOfBirth))
            return BadRequest(SharedProblemDetailsFactory.Create(
                StatusCodes.Status400BadRequest,
                localizer["InvalidDateOfBirthFormatTitle"].Value,
                localizer["InvalidDateOfBirthFormat"].Value,
                HttpContext.Request.Path));

        try
        {
            var command = RegisterBodyMetricsCommandAssembler.ToCommand(resource, dateOfBirth);
            var result = await commandService.Handle(command);
            return result switch
            {
                Result<BodyMetrics, BodyHealthMetricsError>.Success s =>
                    CreatedAtAction(nameof(GetByUserId), new { userId = s.Value.UserId },
                        BodyMetricsResourceAssembler.ToResource(s.Value)),
                Result<BodyMetrics, BodyHealthMetricsError>.Failure f =>
                    BodyHealthMetricsActionResultAssembler.ToActionResult(f.Error, localizer, HttpContext.Request.Path),
                _ => StatusCode(StatusCodes.Status500InternalServerError)
            };
        }
        catch (ArgumentException ex)
        {
            return BadRequest(SharedProblemDetailsFactory.Create(
                StatusCodes.Status400BadRequest,
                localizer["InvalidBodyMetricsDataTitle"].Value,
                ex.Message,
                HttpContext.Request.Path));
        }
    }

    [HttpPut("{userId:int}/weight")]
    [SwaggerOperation(
        Summary = "Update weight for a user",
        Description = "Updates current weight and optional note. Returns updated body metrics with recalculated BMI and TDEE.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Weight updated successfully. Returns the recalculated body metrics.", typeof(BodyMetricsResource))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "The provided weight value is not valid.", typeof(ProblemDetails))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authentication is required to access this resource.")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "No body metrics were found for this user.", typeof(ProblemDetails))]
    public async Task<IActionResult> UpdateWeight(int userId, [FromBody] UpdateWeightResource resource)
    {
        if (userId != this.GetAuthenticatedUserId()) return Forbid();
        var command = UpdateWeightCommandAssembler.ToCommand(userId, resource);
        var result = await commandService.Handle(command);
        return result switch
        {
            Result<BodyMetrics, BodyHealthMetricsError>.Success s =>
                Ok(BodyMetricsResourceAssembler.ToResource(s.Value)),
            Result<BodyMetrics, BodyHealthMetricsError>.Failure f =>
                BodyHealthMetricsActionResultAssembler.ToActionResult(f.Error, localizer, HttpContext.Request.Path),
            _ => StatusCode(StatusCodes.Status500InternalServerError)
        };
    }

    [HttpPost("{userId:int}/body-measurements")]
    [SwaggerOperation(
        Summary = "Register a body measurement",
        Description = "Records waist and neck circumference measurements. Returns updated body metrics.")]
    [SwaggerResponse(StatusCodes.Status201Created, "Body measurement registered successfully. Returns the updated body metrics.", typeof(BodyMetricsResource))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "The provided measurement values are not valid.", typeof(ProblemDetails))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authentication is required to access this resource.")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "No body metrics were found for this user.", typeof(ProblemDetails))]
    public async Task<IActionResult> RegisterBodyMeasurement(int userId, [FromBody] RegisterBodyMeasurementResource resource)
    {
        if (userId != this.GetAuthenticatedUserId()) return Forbid();
        var command = RegisterBodyMeasurementCommandAssembler.ToCommand(userId, resource);
        var result = await commandService.Handle(command);
        return result switch
        {
            Result<BodyMetrics, BodyHealthMetricsError>.Success s =>
                StatusCode(StatusCodes.Status201Created, BodyMetricsResourceAssembler.ToResource(s.Value)),
            Result<BodyMetrics, BodyHealthMetricsError>.Failure f =>
                BodyHealthMetricsActionResultAssembler.ToActionResult(f.Error, localizer, HttpContext.Request.Path),
            _ => StatusCode(StatusCodes.Status500InternalServerError)
        };
    }

    [HttpPut("{userId:int}/health-goal")]
    [SwaggerOperation(
        Summary = "Set a health goal for a user",
        Description = "Sets a new weight goal with target and weekly rate. Returns updated body metrics with adjusted caloric values.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Health goal set successfully. Returns the body metrics with adjusted caloric values.", typeof(BodyMetricsResource))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "The provided goal data is not valid.", typeof(ProblemDetails))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authentication is required to access this resource.")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "No body metrics were found for this user.", typeof(ProblemDetails))]
    public async Task<IActionResult> SetHealthGoal(int userId, [FromBody] SetHealthGoalResource resource)
    {
        if (userId != this.GetAuthenticatedUserId()) return Forbid();
        var command = SetHealthGoalCommandAssembler.ToCommand(userId, resource);
        var result = await commandService.Handle(command);
        return result switch
        {
            Result<BodyMetrics, BodyHealthMetricsError>.Success s =>
                Ok(BodyMetricsResourceAssembler.ToResource(s.Value)),
            Result<BodyMetrics, BodyHealthMetricsError>.Failure f =>
                BodyHealthMetricsActionResultAssembler.ToActionResult(f.Error, localizer, HttpContext.Request.Path),
            _ => StatusCode(StatusCodes.Status500InternalServerError)
        };
    }

    [HttpGet("by-user/{userId:int}")]
    [SwaggerOperation(
        Summary = "Get body metrics by user ID",
        Description = "Retrieves the complete body metrics profile for a user, including biometrics and computed health values.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Body metrics retrieved successfully (body is null when the user has no profile yet).", typeof(BodyMetricsResource))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authentication is required to access this resource.")]
    public async Task<IActionResult> GetByUserId(int userId)
    {
        if (userId != this.GetAuthenticatedUserId()) return Forbid();
        var bodyMetrics = await queryService.Handle(new GetBodyMetricsByUserIdQuery(userId));
        return bodyMetrics is null ? Ok((object?)null) : Ok(BodyMetricsResourceAssembler.ToResource(bodyMetrics));
    }

    [HttpGet("{userId:int}/bmi")]
    [SwaggerOperation(
        Summary = "Get the current BMI for a user",
        Description = "Returns the user's current BMI value and WHO weight-status category (Underweight, Normal, Overweight, Obese).")]
    [SwaggerResponse(StatusCodes.Status200OK, "BMI retrieved successfully.", typeof(BmiResource))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authentication is required to access this resource.")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "No body metrics exist for this user, or the BMI has not been calculated yet.", typeof(ProblemDetails))]
    public async Task<IActionResult> GetBmi(int userId)
    {
        if (userId != this.GetAuthenticatedUserId()) return Forbid();
        var bodyMetrics = await queryService.Handle(new GetBodyMetricsByUserIdQuery(userId));
        if (bodyMetrics is null)
            return NotFound(SharedProblemDetailsFactory.Create(
                StatusCodes.Status404NotFound,
                localizer["BodyMetricsNotFoundTitle"].Value,
                localizer["BodyMetricsNotFound"].Value,
                HttpContext.Request.Path));
        if (bodyMetrics.BmiValue is null || bodyMetrics.BmiCategory is null)
            return NotFound(SharedProblemDetailsFactory.Create(
                StatusCodes.Status404NotFound,
                localizer["BmiNotCalculatedTitle"].Value,
                localizer["BmiNotCalculated"].Value,
                HttpContext.Request.Path));
        return Ok(new BmiResource(bodyMetrics.BmiValue.Value, bodyMetrics.BmiCategory));
    }

    [HttpGet("{userId:int}/daily-caloric-goal")]
    [SwaggerOperation(
        Summary = "Get the daily caloric goal",
        Description = "Returns daily caloric target and macronutrient breakdown (protein, carbs, fat, fiber) in grams.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Daily caloric goal retrieved successfully.", typeof(DailyCaloricGoalResource))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authentication is required to access this resource.")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "No body metrics exist for this user, or the daily caloric goal has not been calculated yet.", typeof(ProblemDetails))]
    public async Task<IActionResult> GetDailyCaloricGoal(int userId)
    {
        if (userId != this.GetAuthenticatedUserId()) return Forbid();
        var bodyMetrics = await queryService.Handle(new GetBodyMetricsByUserIdQuery(userId));
        if (bodyMetrics is null)
            return NotFound(SharedProblemDetailsFactory.Create(
                StatusCodes.Status404NotFound,
                localizer["BodyMetricsNotFoundTitle"].Value,
                localizer["BodyMetricsNotFound"].Value,
                HttpContext.Request.Path));
        if (bodyMetrics.MacroCalories is null)
            return NotFound(SharedProblemDetailsFactory.Create(
                StatusCodes.Status404NotFound,
                localizer["DailyCaloricGoalNotCalculatedTitle"].Value,
                localizer["DailyCaloricGoalNotCalculated"].Value,
                HttpContext.Request.Path));
        return Ok(new DailyCaloricGoalResource(
            bodyMetrics.MacroCalories.Value,
            bodyMetrics.MacroProteinG!.Value,
            bodyMetrics.MacroCarbsG!.Value,
            bodyMetrics.MacroFatG!.Value,
            bodyMetrics.MacroFiberG!.Value));
    }

    [HttpGet("{userId:int}/weight-history")]
    [SwaggerOperation(
        Summary = "Get weight history for a user",
        Description = "Returns paginated weight log entries within optional date range (from/to in ISO 8601 format).")]
    [SwaggerResponse(StatusCodes.Status200OK, "Weight history retrieved successfully.", typeof(WeightLogResource[]))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authentication is required to access this resource.")]
    public async Task<IActionResult> GetWeightHistory(
        int userId,
        [FromQuery] DateTimeOffset? from,
        [FromQuery] DateTimeOffset? to)
    {
        if (userId != this.GetAuthenticatedUserId()) return Forbid();
        var logs = await queryService.Handle(new GetWeightHistoryByUserIdQuery(userId, from, to));
        return Ok(logs.Select(WeightLogResourceAssembler.ToResource));
    }
}
