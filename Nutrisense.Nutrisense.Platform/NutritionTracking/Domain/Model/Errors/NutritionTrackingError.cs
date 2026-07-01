namespace Nutrisense.Nutrisense.Platform.NutritionTracking.Domain.Model.Errors;

public enum NutritionTrackingError
{
    FoodNotFound,
    DetectedFoodNotFound,
    InvalidScanConfirmationData,
    InvalidMealType,
    InvalidMealQuantity,
    InvalidMenuSelectionData,
    DishScanFailed,
    MenuScanFailed,
    EntryNotFound,
    EntryDeleteForbidden,
    EntryUpdateForbidden,
    InvalidEntryQuantity,
    FoodDuplicateKey,
    InvalidFoodSource,
    UsdaUnavailable,
    UnexpectedError
}
