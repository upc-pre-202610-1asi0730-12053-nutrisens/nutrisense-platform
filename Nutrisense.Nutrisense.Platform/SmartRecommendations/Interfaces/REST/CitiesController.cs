using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nutrisense.Nutrisense.Platform.SmartRecommendations.Application.CommandServices;
using Nutrisense.Nutrisense.Platform.SmartRecommendations.Application.QueryServices;
using Nutrisense.Nutrisense.Platform.SmartRecommendations.Domain.Model.Commands;
using Nutrisense.Nutrisense.Platform.SmartRecommendations.Domain.Model.Queries;
using Nutrisense.Nutrisense.Platform.SmartRecommendations.Interfaces.REST.Resources;
using Nutrisense.Nutrisense.Platform.SmartRecommendations.Interfaces.REST.Transform;
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
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var cities = await queryService.Handle(new GetAllCitiesQuery());
        return Ok(cities.Select(CityAssembler.ToResource));
    }

    [HttpGet("search")]
    [AllowAnonymous]
    [SwaggerOperation(Summary = "Search cities by name", Description = "Search cities from the catalog or geocoding service candidates.")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Search([FromQuery] string q, [FromQuery] int limit = 5)
    {
        var results = await queryService.Handle(new SearchCitiesQuery(q ?? "", limit));
        return Ok(results.Select(CityAssembler.ToSearchResource));
    }

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    [SwaggerOperation(Summary = "Get city by ID", Description = "Retrieve a specific city from the catalog by ID.")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var city = await queryService.Handle(new GetCityByIdQuery(id));
        return city is null ? NotFound() : Ok(CityAssembler.ToResource(city));
    }

    [HttpPost]
    [Authorize]
    [Consumes("application/json")]
    [SwaggerOperation(Summary = "Import city into catalog", Description = "Import or reuse a geocoded city into the catalog and return it with an ID.")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Import([FromBody] ImportCityResource resource)
    {
        var result = await commandService.Handle(new ImportCityCommand(
            resource.Name, resource.NameEn, resource.NameEs, resource.Country, resource.Lat, resource.Lng));
        return result.Fold(
            city => (IActionResult)Ok(CityAssembler.ToResource(city)),
            error => UnprocessableEntity(new { error = error.ToString() }));
    }

    [HttpGet("{id:int}/weather")]
    [SwaggerOperation(Summary = "Get current weather", Description = "Retrieve current weather conditions for a city.")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetWeather(int id)
    {
        var snapshot = await queryService.Handle(new GetCurrentWeatherByCityIdQuery(id));
        return snapshot is null ? NotFound() : Ok(WeatherAssembler.ToResource(id, snapshot));
    }
}
