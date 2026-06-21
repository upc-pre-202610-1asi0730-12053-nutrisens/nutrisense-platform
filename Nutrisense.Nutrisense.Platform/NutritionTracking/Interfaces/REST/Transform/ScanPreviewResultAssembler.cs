using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Nutrisense.Nutrisense.Platform.NutritionTracking.Application.Internal;
using Nutrisense.Nutrisense.Platform.NutritionTracking.Application.Errors;
using Nutrisense.Nutrisense.Platform.NutritionTracking.Interfaces.REST.Resources;
using Nutrisense.Nutrisense.Platform.Shared.Application.Patterns;
using Nutrisense.Nutrisense.Platform.Shared.Interfaces.REST.Resources;
using Nutrisense.Nutrisense.Platform.NutritionTracking.Resources;

namespace Nutrisense.Nutrisense.Platform.NutritionTracking.Interfaces.REST.Transform;

public static class ScanPreviewResultAssembler
{
    public static IActionResult ToActionResult(
        Result<ScanPreviewResult, ScanMealPhotoError> result,
        IStringLocalizer<NutritionTrackingMessages> localizer) =>
        result switch
        {
            Result<ScanPreviewResult, ScanMealPhotoError>.Success s =>
                new OkObjectResult(new ScanPreviewResource(
                    s.Value.Items.Select(i => new ScannedDishItemResource(
                        i.FoodId, i.NameEn, i.NameEs, i.EstimatedQuantityG,
                        i.CaloriesPer100g, i.ProteinPer100g, i.CarbsPer100g, i.FatPer100g,
                        i.IsEstimate)))),
            Result<ScanPreviewResult, ScanMealPhotoError>.Failure { Error: ScanMealPhotoError.ScanFailed } =>
                new UnprocessableEntityObjectResult(new ErrorResponse(localizer["DishScanFailed"].Value)),
            Result<ScanPreviewResult, ScanMealPhotoError>.Failure { Error: ScanMealPhotoError.FoodNotFound } =>
                new NotFoundObjectResult(new ErrorResponse(localizer["DetectedFoodNotFound"].Value)),
            _ =>
                new ObjectResult(new ErrorResponse(localizer["UnexpectedError"].Value))
                    { StatusCode = StatusCodes.Status500InternalServerError }
        };
}
