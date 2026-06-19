using Nutrisense.Nutrisense.Platform.Shared.Domain.Model.Entities;

namespace Nutrisense.Nutrisense.Platform.IAM.Domain.Model.Aggregates;

public partial class User : IAuditableEntity
{
    public DateTimeOffset? CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}
