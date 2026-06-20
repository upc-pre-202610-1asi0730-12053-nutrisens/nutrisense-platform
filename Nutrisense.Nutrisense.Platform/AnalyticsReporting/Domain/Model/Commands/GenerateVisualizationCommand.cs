namespace Nutrisense.Nutrisense.Platform.AnalyticsReporting.Domain.Model.Commands;

/// <summary>Request to (re)build the chart and dashboard visualizations for a user.</summary>
public record GenerateVisualizationCommand(int UserId);
