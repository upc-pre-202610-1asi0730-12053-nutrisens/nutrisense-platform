using Nutrisense.Nutrisense.Platform.Shared.Domain.Model.Events;

namespace Nutrisense.Nutrisense.Platform.AnalyticsReporting.Domain.Model.Events;

/// <summary>Raised when a user successfully exports a downloadable report file.</summary>
public record ReportExported(int UserId, string FileName) : DomainEventBase;
