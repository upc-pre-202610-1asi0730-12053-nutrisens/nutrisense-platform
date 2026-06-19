using System.Text.Json.Serialization;

namespace Nutrisense.Nutrisense.Platform.Shared.Infrastructure.External.Gemini;

/// <summary>Request/response DTOs for the Gemini <c>generateContent</c> (v1beta) API.</summary>
public sealed record GeminiRequest(
    [property: JsonPropertyName("contents")] List<GeminiContent> Contents);

public sealed record GeminiContent(
    [property: JsonPropertyName("parts")] List<GeminiPart> Parts);

public sealed record GeminiPart(
    [property: JsonPropertyName("text")] string? Text = null,
    [property: JsonPropertyName("inline_data")] GeminiInlineData? InlineData = null)
{
    public static GeminiPart FromText(string text) => new(Text: text);

    public static GeminiPart FromImage(string base64, string mimeType = "image/jpeg") =>
        new(InlineData: new GeminiInlineData(mimeType, base64));
}

public sealed record GeminiInlineData(
    [property: JsonPropertyName("mime_type")] string MimeType,
    [property: JsonPropertyName("data")] string Data);

public sealed record GeminiResponse(
    [property: JsonPropertyName("candidates")] List<GeminiCandidate>? Candidates);

public sealed record GeminiCandidate(
    [property: JsonPropertyName("content")] GeminiContent? Content);
