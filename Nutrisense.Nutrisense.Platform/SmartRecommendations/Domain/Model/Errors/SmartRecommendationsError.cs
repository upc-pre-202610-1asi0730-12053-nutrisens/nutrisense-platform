namespace Nutrisense.Nutrisense.Platform.SmartRecommendations.Domain.Model.Errors;

public enum SmartRecommendationsError
{
    DetectionFailed,
    LocationPreferenceNotFound,
    CityNotFound,
    TravelModePlanNotSufficient,
    PantryPlanNotSufficient,
    NoFoodsAvailable,
    InvalidCityData,
    InsufficientIngredients,
    IngredientNotFound,
    PantryItemNotFound,
    NoRecipesAvailable,
    UnexpectedError
}
