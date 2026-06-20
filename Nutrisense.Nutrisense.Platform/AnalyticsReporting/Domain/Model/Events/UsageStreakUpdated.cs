using Nutrisense.Nutrisense.Platform.Shared.Domain.Model.Events;

namespace Nutrisense.Nutrisense.Platform.AnalyticsReporting.Domain.Model.Events;

/// <summary>Raised when a user's consecutive-day logging streak changes.</summary>
public record UsageStreakUpdated(int UserId, int CurrentStreak) : DomainEventBase;
