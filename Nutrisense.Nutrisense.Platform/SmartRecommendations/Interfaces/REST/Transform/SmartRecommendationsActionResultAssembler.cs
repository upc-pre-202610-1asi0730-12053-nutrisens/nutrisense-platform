using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Nutrisense.Nutrisense.Platform.Shared.Interfaces.REST.ProblemDetails;
using Nutrisense.Nutrisense.Platform.SmartRecommendations.Domain.Model.Errors;
using Nutrisense.Nutrisense.Platform.SmartRecommendations.Resources;

namespace Nutrisense.Nutrisense.Platform.SmartRecommendations.Interfaces.REST.Transform;

/// <summary>Maps a <see cref="SmartRecommendationsError"/> to its RFC 7807 HTTP response, centralizing the enum-to-status switch for the whole context.</summary>
public static class SmartRecommendationsActionResultAssembler
{
    public static IActionResult ToActionResult(
        SmartRecommendationsError error,
        IStringLocalizer<SmartRecommendationsMessages> localizer,
        string? instance = null)
    {
        var (status, titleKey, detailKey) = error switch
        {
            SmartRecommendationsError.DetectionFailed =>
                (StatusCodes.Status422UnprocessableEntity, "Unprocessable Entity", "LocationDetectionFailed"),
            SmartRecommendationsError.LocationPreferenceNotFound =>
                (StatusCodes.Status404NotFound, "Not Found", "LocationPreferencesNotFound"),
            SmartRecommendationsError.CityNotFound =>
                (StatusCodes.Status404NotFound, "Not Found", "CityNotFound"),
            SmartRecommendationsError.TravelModePlanNotSufficient =>
                (StatusCodes.Status402PaymentRequired, "Payment Required", "PlanDoesNotAllowTravelMode"),
            SmartRecommendationsError.PantryPlanNotSufficient =>
                (StatusCodes.Status402PaymentRequired, "Payment Required", "PlanDoesNotAllowMorePantryItems"),
            SmartRecommendationsError.NoFoodsAvailable =>
                (StatusCodes.Status422UnprocessableEntity, "Unprocessable Entity", "RecommendationGenerationFailed"),
            SmartRecommendationsError.InvalidCityData =>
                (StatusCodes.Status422UnprocessableEntity, "Unprocessable Entity", "CityImportFailed"),
            SmartRecommendationsError.InsufficientIngredients =>
                (StatusCodes.Status422UnprocessableEntity, "Unprocessable Entity", "InsufficientIngredientsForRecipes"),
            SmartRecommendationsError.IngredientNotFound =>
                (StatusCodes.Status422UnprocessableEntity, "Unprocessable Entity", "PantryItemsRegistrationFailed"),
            SmartRecommendationsError.PantryItemNotFound =>
                (StatusCodes.Status404NotFound, "Not Found", "PantryItemNotFound"),
            SmartRecommendationsError.NoRecipesAvailable =>
                (StatusCodes.Status422UnprocessableEntity, "Unprocessable Entity", "RecipeSuggestionFailed"),
            _ =>
                (StatusCodes.Status500InternalServerError, "Internal Server Error", "UnexpectedError")
        };

        var problem = ProblemDetailsFactory.Create(status, titleKey, localizer[detailKey].Value, instance);
        return new ObjectResult(problem) { StatusCode = status };
    }
}
