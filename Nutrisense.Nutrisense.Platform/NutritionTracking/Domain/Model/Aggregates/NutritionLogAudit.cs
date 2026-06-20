using Nutrisense.Nutrisense.Platform.Shared.Domain.Model.Entities;

namespace Nutrisense.Nutrisense.Platform.NutritionTracking.Domain.Model.Aggregates;

public partial class NutritionLog : IAuditableEntity
{
    public DateTimeOffset? CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}
