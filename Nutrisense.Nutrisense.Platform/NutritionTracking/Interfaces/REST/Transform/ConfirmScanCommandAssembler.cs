using Nutrisense.Nutrisense.Platform.NutritionTracking.Domain.Model.Commands;
using Nutrisense.Nutrisense.Platform.NutritionTracking.Interfaces.REST.Resources;

namespace Nutrisense.Nutrisense.Platform.NutritionTracking.Interfaces.REST.Transform;

public static class ConfirmScanCommandAssembler
{
    public static ConfirmScanResultCommand ToCommand(ConfirmScanResource resource) =>
        new(resource.UserId, resource.DetectedFoodId, resource.QuantityG,
            resource.MealType,
            DateOnly.ParseExact(resource.Date, "yyyy-MM-dd", null));
}
