using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Nutrisense.Nutrisense.Platform.NutritionTracking.Application.Internal;
using Nutrisense.Nutrisense.Platform.NutritionTracking.Application.Errors;
using Nutrisense.Nutrisense.Platform.NutritionTracking.Interfaces.REST.Resources;
using Nutrisense.Nutrisense.Platform.Shared.Application.Patterns;
using Nutrisense.Nutrisense.Platform.Shared.Interfaces.REST.Resources;
using Nutrisense.Nutrisense.Platform.Shared.Resources;

namespace Nutrisense.Nutrisense.Platform.NutritionTracking.Interfaces.REST.Transform;

public static class MenuOptionsPreviewAssembler
{
    public static IActionResult ToActionResult(
        Result<MenuOptionsPreview, ScanMenuPhotoError> result,
        IStringLocalizer<SharedResource> localizer) =>
        result switch
        {
            Result<MenuOptionsPreview, ScanMenuPhotoError>.Success s =>
                new OkObjectResult(new MenuOptionsPreviewResource(
                    s.Value.Options.Select(o => new MenuOptionResource(
                        o.FoodId, o.NameEn, o.NameEs,
                        o.CaloriesPer100g, o.ProteinPer100g, o.CarbsPer100g, o.FatPer100g,
                        o.Restrictions, o.IsEstimate)))),
            Result<MenuOptionsPreview, ScanMenuPhotoError>.Failure { Error: ScanMenuPhotoError.ScanFailed } =>
                new UnprocessableEntityObjectResult(new ErrorResponse("Menu scan failed.")),
            _ =>
                new ObjectResult(new ErrorResponse(localizer["UnexpectedError"].Value))
                    { StatusCode = StatusCodes.Status500InternalServerError }
        };
}
