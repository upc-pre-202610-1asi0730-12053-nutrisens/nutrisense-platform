using System.Text;
using Nutrisense.Nutrisense.Platform.AnalyticsReporting.Domain.Model.ValueObjects;
using Nutrisense.Nutrisense.Platform.AnalyticsReporting.Domain.Services;

namespace Nutrisense.Nutrisense.Platform.AnalyticsReporting.Infrastructure.External.Pdf;

/// <summary>Placeholder PDF report generator producing a minimal document for a user's date range.</summary>
public class PdfReportGenerator : IReportPdfGenerator
{
    // TODO: implement real PDF generation (e.g. QuestPDF or iTextSharp)
    public Task<byte[]> GenerateAsync(int userId, DateRange range, CancellationToken ct = default)
    {
        var content =
            $"%PDF-1.4\n" +
            $"1 0 obj<</Type/Catalog/Pages 2 0 R>>endobj\n" +
            $"2 0 obj<</Type/Pages/Kids[3 0 R]/Count 1>>endobj\n" +
            $"3 0 obj<</Type/Page/MediaBox[0 0 612 792]/Parent 2 0 R>>endobj\n" +
            $"xref\n0 4\n0000000000 65535 f\n" +
            $"trailer<</Size 4/Root 1 0 R>>\n" +
            $"startxref\n0\n%%EOF\n" +
            $"% Report for user {userId} | {range.From:yyyy-MM-dd} to {range.To:yyyy-MM-dd}";

        return Task.FromResult(Encoding.ASCII.GetBytes(content));
    }
}
