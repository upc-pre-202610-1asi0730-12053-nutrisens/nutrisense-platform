namespace Nutrisense.Nutrisense.Platform.Shared.Domain.Model.Events;

public abstract record DomainEventBase : IEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTimeOffset OccurredAt { get; init; } = DateTimeOffset.UtcNow;
}
