using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Nutrisense.Nutrisense.Platform.NutritionTracking.Application.Internal;
using Nutrisense.Nutrisense.Platform.NutritionTracking.Domain.Model.Aggregates;
using Nutrisense.Nutrisense.Platform.NutritionTracking.Domain.Model.Errors;
using Nutrisense.Nutrisense.Platform.NutritionTracking.Interfaces.REST.Resources;
using Nutrisense.Nutrisense.Platform.NutritionTracking.Resources;
using Nutrisense.Nutrisense.Platform.Shared.Application.Patterns;
using ProblemDetailsFactory = Nutrisense.Nutrisense.Platform.Shared.Interfaces.REST.ProblemDetails.ProblemDetailsFactory;

namespace Nutrisense.Nutrisense.Platform.NutritionTracking.Interfaces.REST.Transform;

/// <summary>Centralizes NutritionTrackingError → HTTP problem-details mapping for all NutritionTracking result assemblers.</summary>
public static class NutritionTrackingActionResultAssembler
{
    public static IActionResult ToCreatedActionResult(
        Result<NutritionLog, NutritionTrackingError> result,
        IStringLocalizer<NutritionTrackingMessages> localizer,
        string? instance = null) =>
        result switch
        {
            Result<NutritionLog, NutritionTrackingError>.Success s =>
                new ObjectResult(NutritionLogResourceAssembler.ToResource(s.Value))
                    { StatusCode = StatusCodes.Status201Created },
            Result<NutritionLog, NutritionTrackingError>.Failure f => ToProblemResult(f.Error, localizer, instance),
            _ => ToProblemResult(NutritionTrackingError.UnexpectedError, localizer, instance)
        };

    public static IActionResult ToUpdatedActionResult(
        Result<NutritionLog, NutritionTrackingError> result,
        IStringLocalizer<NutritionTrackingMessages> localizer,
        string? instance = null) =>
        result switch
        {
            Result<NutritionLog, NutritionTrackingError>.Success s =>
                new OkObjectResult(NutritionLogResourceAssembler.ToResource(s.Value)),
            Result<NutritionLog, NutritionTrackingError>.Failure f => ToProblemResult(f.Error, localizer, instance),
            _ => ToProblemResult(NutritionTrackingError.UnexpectedError, localizer, instance)
        };

    public static IActionResult ToDeletedActionResult(
        Result<bool, NutritionTrackingError> result,
        IStringLocalizer<NutritionTrackingMessages> localizer,
        string? instance = null) =>
        result switch
        {
            Result<bool, NutritionTrackingError>.Success => new NoContentResult(),
            Result<bool, NutritionTrackingError>.Failure f => ToProblemResult(f.Error, localizer, instance),
            _ => ToProblemResult(NutritionTrackingError.UnexpectedError, localizer, instance)
        };

    public static IActionResult ToScanPreviewActionResult(
        Result<ScanPreviewResult, NutritionTrackingError> result,
        IStringLocalizer<NutritionTrackingMessages> localizer,
        string? instance = null) =>
        result switch
        {
            Result<ScanPreviewResult, NutritionTrackingError>.Success s =>
                new OkObjectResult(new ScanPreviewResource(
                    s.Value.Items.Select(i => new ScannedDishItemResource(
                        i.FoodId, i.NameEn, i.NameEs, i.EstimatedQuantityG,
                        i.CaloriesPer100g, i.ProteinPer100g, i.CarbsPer100g, i.FatPer100g,
                        i.IsEstimate)))),
            Result<ScanPreviewResult, NutritionTrackingError>.Failure f => ToProblemResult(f.Error, localizer, instance),
            _ => ToProblemResult(NutritionTrackingError.UnexpectedError, localizer, instance)
        };

    public static IActionResult ToMenuPreviewActionResult(
        Result<MenuOptionsPreview, NutritionTrackingError> result,
        IStringLocalizer<NutritionTrackingMessages> localizer,
        string? instance = null) =>
        result switch
        {
            Result<MenuOptionsPreview, NutritionTrackingError>.Success s =>
                new OkObjectResult(new MenuOptionsPreviewResource(
                    s.Value.Options.Select(o => new MenuOptionResource(
                        o.FoodId, o.NameEn, o.NameEs,
                        o.CaloriesPer100g, o.ProteinPer100g, o.CarbsPer100g, o.FatPer100g,
                        o.Restrictions, o.IsEstimate)))),
            Result<MenuOptionsPreview, NutritionTrackingError>.Failure f => ToProblemResult(f.Error, localizer, instance),
            _ => ToProblemResult(NutritionTrackingError.UnexpectedError, localizer, instance)
        };

    private static ObjectResult ToProblemResult(
        NutritionTrackingError error,
        IStringLocalizer<NutritionTrackingMessages> localizer,
        string? instance)
    {
        var (status, title, detail) = error switch
        {
            NutritionTrackingError.FoodNotFound =>
                (StatusCodes.Status404NotFound, localizer["FoodNotFoundTitle"].Value, localizer["FoodNotFound"].Value),
            NutritionTrackingError.DetectedFoodNotFound =>
                (StatusCodes.Status404NotFound, localizer["FoodNotFoundTitle"].Value, localizer["DetectedFoodNotFound"].Value),
            NutritionTrackingError.EntryNotFound =>
                (StatusCodes.Status404NotFound, localizer["NutritionLogEntryNotFoundTitle"].Value, localizer["NutritionLogEntryNotFound"].Value),
            NutritionTrackingError.EntryDeleteForbidden =>
                (StatusCodes.Status403Forbidden, localizer["Unauthorized"].Value, localizer["NutritionLogEntryDeleteUnauthorized"].Value),
            NutritionTrackingError.EntryUpdateForbidden =>
                (StatusCodes.Status403Forbidden, localizer["Unauthorized"].Value, localizer["NutritionLogEntryUnauthorized"].Value),
            NutritionTrackingError.InvalidScanConfirmationData =>
                (StatusCodes.Status400BadRequest, localizer["BadRequestTitle"].Value, localizer["InvalidScanConfirmationData"].Value),
            NutritionTrackingError.InvalidMealType =>
                (StatusCodes.Status400BadRequest, localizer["BadRequestTitle"].Value, localizer["InvalidMealType"].Value),
            NutritionTrackingError.InvalidMealQuantity =>
                (StatusCodes.Status400BadRequest, localizer["BadRequestTitle"].Value, localizer["InvalidQuantity"].Value),
            NutritionTrackingError.InvalidEntryQuantity =>
                (StatusCodes.Status400BadRequest, localizer["BadRequestTitle"].Value, localizer["NutritionLogEntryInvalidQuantity"].Value),
            NutritionTrackingError.InvalidMenuSelectionData =>
                (StatusCodes.Status400BadRequest, localizer["BadRequestTitle"].Value, localizer["InvalidMenuSelectionData"].Value),
            NutritionTrackingError.InvalidFoodSource =>
                (StatusCodes.Status400BadRequest, localizer["BadRequestTitle"].Value, localizer["InvalidFoodSource"].Value),
            NutritionTrackingError.FoodDuplicateKey =>
                (StatusCodes.Status409Conflict, localizer["ConflictTitle"].Value, localizer["FoodDuplicateKey"].Value),
            NutritionTrackingError.DishScanFailed =>
                (StatusCodes.Status422UnprocessableEntity, localizer["UnprocessableEntityTitle"].Value, localizer["DishScanFailed"].Value),
            NutritionTrackingError.MenuScanFailed =>
                (StatusCodes.Status422UnprocessableEntity, localizer["UnprocessableEntityTitle"].Value, localizer["MenuScanFailed"].Value),
            NutritionTrackingError.UsdaUnavailable =>
                (StatusCodes.Status422UnprocessableEntity, localizer["UnprocessableEntityTitle"].Value, localizer["UsdaServiceUnavailable"].Value),
            _ =>
                (StatusCodes.Status500InternalServerError, localizer["UnexpectedServerError"].Value, localizer["UnexpectedError"].Value)
        };

        return new ObjectResult(ProblemDetailsFactory.Create(status, title, detail, instance)) { StatusCode = status };
    }
}
