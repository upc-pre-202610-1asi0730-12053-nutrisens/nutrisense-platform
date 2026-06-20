using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nutrisense.Nutrisense.Platform.SmartRecommendations.Application.CommandServices;
using Nutrisense.Nutrisense.Platform.SmartRecommendations.Application.QueryServices;
using Nutrisense.Nutrisense.Platform.SmartRecommendations.Domain.Model.Commands;
using Nutrisense.Nutrisense.Platform.SmartRecommendations.Domain.Model.Queries;
using Nutrisense.Nutrisense.Platform.SmartRecommendations.Interfaces.REST.Resources;
using Nutrisense.Nutrisense.Platform.SmartRecommendations.Interfaces.REST.Transform;
using Nutrisense.Nutrisense.Platform.Shared.Interfaces.REST.Resources;
using Swashbuckle.AspNetCore.Annotations;

namespace Nutrisense.Nutrisense.Platform.SmartRecommendations.Interfaces.REST;

[ApiController]
[Route("api/v1/cities")]
[Tags("Cities")]
[Produces("application/json")]
public class CitiesController(
    IRecsEngineQueryService queryService,
    IRecsEngineCommandService commandService) : ControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    [SwaggerOperation(Summary = "List all cities", Description = "Retrieve all cities from the catalog.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Cities retrieved successfully.", typeof(CityResource[]))]
    public async Task<IActionResult> GetAll()
    {
        var cities = await queryService.Handle(new GetAllCitiesQuery());
        return Ok(cities.Select(CityAssembler.ToResource));
    }

    [HttpGet("search")]
    [AllowAnonymous]
    [SwaggerOperation(Summary = "Search cities by name", Description = "Search cities from the catalog or geocoding service candidates.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Matching cities retrieved successfully.", typeof(CitySearchResultResource[]))]
    public async Task<IActionResult> Search([FromQuery] string q, [FromQuery] int limit = 5)
    {
        var results = await queryService.Handle(new SearchCitiesQuery(q ?? "", limit));
        return Ok(results.Select(CityAssembler.ToSearchResource));
    }

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    [SwaggerOperation(Summary = "Get city by ID", Description = "Retrieve a specific city from the catalog by ID.")]
    [SwaggerResponse(StatusCodes.Status200OK, "City retrieved successfully.", typeof(CityResource))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "No city was found with the specified id.")]
    public async Task<IActionResult> GetById(int id)
    {
        var city = await queryService.Handle(new GetCityByIdQuery(id));
        return city is null ? NotFound() : Ok(CityAssembler.ToResource(city));
    }

    [HttpPost]
    [Authorize]
    [Consumes("application/json")]
    [SwaggerOperation(Summary = "Import city into catalog", Description = "Import or reuse a geocoded city into the catalog and return it with an ID.")]
    [SwaggerResponse(StatusCodes.Status200OK, "City imported successfully.", typeof(CityResource))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authentication is required to access this resource.")]
    [SwaggerResponse(StatusCodes.Status422UnprocessableEntity, "The city could not be imported from the provided data.", typeof(ErrorResponse))]
    public async Task<IActionResult> Import([FromBody] ImportCityResource resource)
    {
        var result = await commandService.Handle(new ImportCityCommand(
            resource.Name, resource.NameEn, resource.NameEs, resource.Country, resource.Lat, resource.Lng));
        return result.Fold(
            city => (IActionResult)Ok(CityAssembler.ToResource(city)),
            error => UnprocessableEntity(new ErrorResponse("The city could not be imported from the provided data.")));
    }

    [HttpGet("{id:int}/weather")]
    [SwaggerOperation(Summary = "Get current weather", Description = "Retrieve current weather conditions for a city.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Current weather retrieved successfully.", typeof(WeatherResource))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "No weather data was found for the specified city.")]
    public async Task<IActionResult> GetWeather(int id)
    {
        var snapshot = await queryService.Handle(new GetCurrentWeatherByCityIdQuery(id));
        return snapshot is null ? NotFound() : Ok(WeatherAssembler.ToResource(id, snapshot));
    }
}
