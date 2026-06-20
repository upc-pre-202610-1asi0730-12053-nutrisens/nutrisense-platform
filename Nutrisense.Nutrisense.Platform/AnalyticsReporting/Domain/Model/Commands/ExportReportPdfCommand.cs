namespace Nutrisense.Nutrisense.Platform.AnalyticsReporting.Domain.Model.Commands;

/// <summary>Request to generate a downloadable PDF report covering a date range for a user.</summary>
public record ExportReportPdfCommand(int UserId, DateOnly From, DateOnly To);
