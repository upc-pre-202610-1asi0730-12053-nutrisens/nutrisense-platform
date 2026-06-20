using Nutrisense.Nutrisense.Platform.BodyHealthMetrics.Domain.Model.Entities;
using Nutrisense.Nutrisense.Platform.BodyHealthMetrics.Interfaces.REST.Resources;

namespace Nutrisense.Nutrisense.Platform.BodyHealthMetrics.Interfaces.REST.Transform;

/// <summary>Maps WeightLog entities to WeightLogResource DTOs.</summary>
public static class WeightLogResourceAssembler
{
    public static WeightLogResource ToResource(WeightLog log) =>
        new(log.WeightKg.Value, log.LoggedAt, log.Note);
}
