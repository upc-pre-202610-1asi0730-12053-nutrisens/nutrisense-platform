using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Nutrisense.Nutrisense.Platform.NutritionTracking.Application.Internal;
using Nutrisense.Nutrisense.Platform.NutritionTracking.Application.Errors;
using Nutrisense.Nutrisense.Platform.NutritionTracking.Interfaces.REST.Resources;
using Nutrisense.Nutrisense.Platform.Shared.Application.Patterns;
using Nutrisense.Nutrisense.Platform.Shared.Resources;

namespace Nutrisense.Nutrisense.Platform.NutritionTracking.Interfaces.REST.Transform;

public static class ScanPreviewResultAssembler
{
    public static IActionResult ToActionResult(
        Result<ScanPreviewResult, ScanMealPhotoError> result,
        IStringLocalizer<SharedResource> localizer) =>
        result switch
        {
            Result<ScanPreviewResult, ScanMealPhotoError>.Success s =>
                new OkObjectResult(new ScanPreviewResource(
                    s.Value.Items.Select(i => new ScannedDishItemResource(
                        i.FoodId, i.NameEn, i.NameEs, i.EstimatedQuantityG,
                        i.CaloriesPer100g, i.ProteinPer100g, i.CarbsPer100g, i.FatPer100g,
                        i.IsEstimate)))),
            Result<ScanPreviewResult, ScanMealPhotoError>.Failure { Error: ScanMealPhotoError.ScanFailed } =>
                new UnprocessableEntityObjectResult(new { message = "Dish scan failed." }),
            Result<ScanPreviewResult, ScanMealPhotoError>.Failure { Error: ScanMealPhotoError.FoodNotFound } =>
                new NotFoundObjectResult(new { message = "Detected food not found in catalog." }),
            _ =>
                new ObjectResult(new { message = localizer["UnexpectedError"].Value })
                    { StatusCode = StatusCodes.Status500InternalServerError }
        };
}
