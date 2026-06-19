using System.Net.Http.Json;

namespace Nutrisense.Nutrisense.Platform.Shared.Infrastructure.External.Gemini;

/// <summary>
/// Thin multimodal client over the Gemini <c>generateContent</c> endpoint. Shared across bounded
/// contexts (dish/menu vision). Combines a text prompt with an inline base64 image and returns the
/// raw text the model produced. Reads <c>Gemini:ApiKey</c>, <c>Gemini:BaseUrl</c>, <c>Gemini:Model</c>.
/// </summary>
public class GeminiClient(HttpClient httpClient, IConfiguration configuration)
{
    public async Task<string> GenerateContentAsync(string prompt, string imageBase64OrUri, CancellationToken ct = default)
    {
        var apiKey = configuration["Gemini:ApiKey"];
        if (string.IsNullOrWhiteSpace(apiKey))
            throw new InvalidOperationException("Gemini API key not configured (Gemini:ApiKey).");

        var baseUrl = configuration["Gemini:BaseUrl"]?.TrimEnd('/') ?? "https://generativelanguage.googleapis.com";
        var model = configuration["Gemini:Model"] ?? "gemini-2.5-flash";

        var (mimeType, cleanBase64) = NormalizeImage(imageBase64OrUri);

        var request = new GeminiRequest([
            new GeminiContent([
                GeminiPart.FromText(prompt),
                GeminiPart.FromImage(cleanBase64, mimeType)
            ])
        ]);

        var url = $"{baseUrl}/v1beta/models/{model}:generateContent?key={apiKey}";
        var response = await httpClient.PostAsJsonAsync(url, request, ct);
        response.EnsureSuccessStatusCode();

        var payload = await response.Content.ReadFromJsonAsync<GeminiResponse>(ct);
        var parts = payload?.Candidates?.FirstOrDefault()?.Content?.Parts;
        if (parts is null || parts.Count == 0) return "[]";
        return parts[0].Text ?? "[]";
    }

    /// <summary>
    /// Strips a data-URL prefix (e.g. <c>data:image/png;base64,</c>) if present and infers the MIME
    /// type from it; defaults to <c>image/jpeg</c> for a bare base64 string.
    /// </summary>
    private static (string MimeType, string Base64) NormalizeImage(string image)
    {
        if (string.IsNullOrWhiteSpace(image)) return ("image/jpeg", string.Empty);

        var commaIdx = image.IndexOf(',');
        if (image.StartsWith("data:", StringComparison.OrdinalIgnoreCase) && commaIdx > 0)
        {
            var header = image[5..commaIdx];                       // image/png;base64
            var mime = header.Split(';')[0];
            var data = image[(commaIdx + 1)..];
            return (string.IsNullOrWhiteSpace(mime) ? "image/jpeg" : mime, data);
        }

        return ("image/jpeg", commaIdx >= 0 ? image[(commaIdx + 1)..] : image);
    }
}
