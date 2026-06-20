using Nutrisense.Nutrisense.Platform.NutritionTracking.Domain.Model.Aggregates;
using Nutrisense.Nutrisense.Platform.NutritionTracking.Interfaces.REST.Resources;

namespace Nutrisense.Nutrisense.Platform.NutritionTracking.Interfaces.REST.Transform;

public static class NutritionLogResourceAssembler
{
    public static NutritionLogResource ToResource(NutritionLog log) =>
        new(log.Id, log.UserId, log.FoodId,
            log.FoodNameEn, log.FoodNameEs,
            log.MealType, log.Date.ToString("yyyy-MM-dd"),
            log.QuantityG, log.Calories, log.ProteinG, log.CarbsG,
            log.FatG, log.FiberG, log.SugarG,
            log.Source, log.LoggedAt,
            log.ScanType, log.ScanConfidence, log.ScanImageUri);
}
