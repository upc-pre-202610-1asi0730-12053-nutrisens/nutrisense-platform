using Nutrisense.Nutrisense.Platform.Shared.Domain.Model.Events;

namespace Nutrisense.Nutrisense.Platform.AnalyticsReporting.Domain.Model.Events;

/// <summary>Raised when a user opens their analytics dashboard, signalling engagement activity.</summary>
public record DashboardViewed(int UserId) : DomainEventBase;
