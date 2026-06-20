using Nutrisense.Nutrisense.Platform.Shared.Domain.Model.Entities;

namespace Nutrisense.Nutrisense.Platform.ActivityWearable.Domain.Model.Aggregates;

/// <summary>Auditing partial of <see cref="ActivityLog"/> exposing the persistence timestamps managed by the infrastructure layer.</summary>
public partial class ActivityLog : IAuditableEntity
{
    /// <summary>Instant (UTC) at which the record was first persisted.</summary>
    public DateTimeOffset? CreatedAt { get; set; }

    /// <summary>Instant (UTC) at which the record was last updated.</summary>
    public DateTimeOffset? UpdatedAt { get; set; }
}
