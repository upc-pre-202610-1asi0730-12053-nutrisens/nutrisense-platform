namespace Nutrisense.Nutrisense.Platform.AnalyticsReporting.Application.Errors;

/// <summary>Failure reasons for exporting a PDF report, including access and validation issues.</summary>
public enum ExportReportPdfError
{
    PremiumRequired,
    InvalidDateRange,
    UnexpectedError
}
