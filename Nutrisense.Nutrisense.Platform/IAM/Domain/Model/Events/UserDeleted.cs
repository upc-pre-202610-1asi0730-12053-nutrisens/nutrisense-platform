using Nutrisense.Nutrisense.Platform.IAM.Domain.Model.ValueObjects;
using Nutrisense.Nutrisense.Platform.Shared.Domain.Model.Events;

namespace Nutrisense.Nutrisense.Platform.IAM.Domain.Model.Events;

/// <summary>
/// Published after a user account is permanently deleted from the IAM context.
/// Cross-BC subscribers (BodyMetrics, Subscriptions, SmartRecommendations, Analytics,
/// NutritionTracking, ActivityWearable) should listen to this event to clean up rows
/// that reference UserId without a DB-level FK (cross-boundary references).
/// </summary>
public record UserDeleted(UserId UserId) : DomainEventBase;
