using Nutrisense.Nutrisense.Platform.BodyHealthMetrics.Domain.Model.Commands;
using Nutrisense.Nutrisense.Platform.BodyHealthMetrics.Interfaces.REST.Resources;

namespace Nutrisense.Nutrisense.Platform.BodyHealthMetrics.Interfaces.REST.Transform;

/// <summary>Maps RegisterBodyMetricsResource to RegisterBodyMetricsCommand. The date of birth is parsed and validated by the controller.</summary>
public static class RegisterBodyMetricsCommandAssembler
{
    public static RegisterBodyMetricsCommand ToCommand(RegisterBodyMetricsResource resource, DateOnly dateOfBirth)
    {
        return new RegisterBodyMetricsCommand(
            resource.UserId,
            resource.HeightCm,
            resource.WeightKg,
            dateOfBirth,
            resource.BiologicalSex,
            resource.ActivityLevel,
            resource.Goal,
            resource.WeeklyRateKg);
    }
}
