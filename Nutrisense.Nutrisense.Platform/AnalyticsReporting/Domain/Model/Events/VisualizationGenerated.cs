using Nutrisense.Nutrisense.Platform.Shared.Domain.Model.Events;

namespace Nutrisense.Nutrisense.Platform.AnalyticsReporting.Domain.Model.Events;

/// <summary>Raised when updated chart and dashboard visualizations are ready for a user.</summary>
public record VisualizationGenerated(int UserId) : DomainEventBase;
