namespace Nutrisense.Nutrisense.Platform.Shared.Domain.Model;

/// <summary>
///     Marks an entity as carrying audit timestamps managed by the persistence layer.
/// </summary>
public interface IAuditableEntity
{
    DateTimeOffset? CreatedAt { get; set; }
    DateTimeOffset? UpdatedAt { get; set; }
}
