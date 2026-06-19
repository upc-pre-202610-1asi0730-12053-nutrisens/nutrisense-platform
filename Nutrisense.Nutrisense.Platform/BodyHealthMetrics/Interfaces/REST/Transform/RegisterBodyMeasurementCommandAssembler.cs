using Nutrisense.Nutrisense.Platform.BodyHealthMetrics.Domain.Model.Commands;
using Nutrisense.Nutrisense.Platform.BodyHealthMetrics.Interfaces.REST.Resources;

namespace Nutrisense.Nutrisense.Platform.BodyHealthMetrics.Interfaces.REST.Transform;

/// <summary>Maps RegisterBodyMeasurementResource to RegisterBodyMeasurementCommand.</summary>
public static class RegisterBodyMeasurementCommandAssembler
{
    public static RegisterBodyMeasurementCommand ToCommand(int userId, RegisterBodyMeasurementResource resource) =>
        new(userId, resource.WaistCm, resource.NeckCm);
}
