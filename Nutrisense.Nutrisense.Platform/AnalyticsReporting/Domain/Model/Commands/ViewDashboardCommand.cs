namespace Nutrisense.Nutrisense.Platform.AnalyticsReporting.Domain.Model.Commands;

/// <summary>Request to record that a user viewed their dashboard and update related engagement state.</summary>
public record ViewDashboardCommand(int UserId);
