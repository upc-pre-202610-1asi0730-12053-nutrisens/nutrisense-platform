namespace Nutrisense.Nutrisense.Platform.Shared.Domain.Model.Entities;

public interface IAuditableEntity
{
    DateTimeOffset? CreatedAt { get; set; }
    DateTimeOffset? UpdatedAt { get; set; }
}
