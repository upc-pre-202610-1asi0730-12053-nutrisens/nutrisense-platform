using Nutrisense.Nutrisense.Platform.NutritionTracking.Domain.Model.Commands;
using Nutrisense.Nutrisense.Platform.NutritionTracking.Interfaces.REST.Resources;

namespace Nutrisense.Nutrisense.Platform.NutritionTracking.Interfaces.REST.Transform;

public static class SelectMenuOptionCommandAssembler
{
    public static SelectMenuOptionCommand ToCommand(SelectMenuOptionResource resource) =>
        new(resource.UserId, resource.FoodId, resource.MealType,
            DateOnly.ParseExact(resource.Date, "yyyy-MM-dd", null),
            resource.QuantityG);
}
