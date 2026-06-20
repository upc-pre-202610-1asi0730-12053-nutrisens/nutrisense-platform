using Nutrisense.Nutrisense.Platform.BodyHealthMetrics.Domain.Model.Commands;
using Nutrisense.Nutrisense.Platform.BodyHealthMetrics.Interfaces.REST.Resources;

namespace Nutrisense.Nutrisense.Platform.BodyHealthMetrics.Interfaces.REST.Transform;

/// <summary>Maps UpdateWeightResource to UpdateWeightCommand.</summary>
public static class UpdateWeightCommandAssembler
{
    public static UpdateWeightCommand ToCommand(int userId, UpdateWeightResource resource) =>
        new(userId, resource.WeightKg, resource.Note);
}
