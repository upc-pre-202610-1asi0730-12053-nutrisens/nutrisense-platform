using Nutrisense.Nutrisense.Platform.Shared.Domain.Model.Events;

namespace Nutrisense.Nutrisense.Platform.ActivityWearable.Domain.Model.Events;

/// <summary>Domain event published when one or more activities have been imported or logged for a user on a day.</summary>
/// <param name="UserId">Identifier of the user the activities belong to.</param>
/// <param name="Date">Calendar day the activities were imported for.</param>
/// <param name="ActivitiesCount">Number of activities imported in this batch.</param>
public record ActivityImported(int UserId, DateOnly Date, int ActivitiesCount) : DomainEventBase;
