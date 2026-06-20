using Nutrisense.Nutrisense.Platform.Shared.Domain.Model.Entities;

namespace Nutrisense.Nutrisense.Platform.Subscriptions.Domain.Model.Aggregates;

public partial class UserSubscription : IAuditableEntity
{
    public DateTimeOffset? CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}
