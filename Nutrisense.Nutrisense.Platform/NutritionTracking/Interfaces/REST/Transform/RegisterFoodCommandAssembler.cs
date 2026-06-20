using Nutrisense.Nutrisense.Platform.NutritionTracking.Domain.Model.Commands;
using Nutrisense.Nutrisense.Platform.NutritionTracking.Interfaces.REST.Resources;

namespace Nutrisense.Nutrisense.Platform.NutritionTracking.Interfaces.REST.Transform;

public static class RegisterFoodCommandAssembler
{
    public static RegisterFoodCommand ToCommand(RegisterFoodResource resource) =>
        new(resource.NameEn, resource.NameEs, resource.Category, resource.Source, resource.ExternalId,
            resource.ServingSizeG, resource.ServingUnit,
            resource.CaloriesPer100g, resource.ProteinPer100g, resource.CarbsPer100g,
            resource.FatPer100g, resource.FiberPer100g, resource.SugarPer100g,
            resource.Restrictions);
}
