using Nutrisense.Nutrisense.Platform.NutritionTracking.Domain.Model.Commands;
using Nutrisense.Nutrisense.Platform.NutritionTracking.Interfaces.REST.Resources;

namespace Nutrisense.Nutrisense.Platform.NutritionTracking.Interfaces.REST.Transform;

public static class UpdateNutritionLogEntryCommandAssembler
{
    public static UpdateNutritionLogEntryCommand ToCommand(int entryId, UpdateNutritionLogResource resource) =>
        new(entryId, resource.UserId, resource.QuantityG);
}
