using System.Text.Json;
using Nutrisense.Nutrisense.Platform.NutritionTracking.Domain.Services;
using Nutrisense.Nutrisense.Platform.Shared.Infrastructure.External.Gemini;

namespace Nutrisense.Nutrisense.Platform.NutritionTracking.Infrastructure.External.Gemini;

/// <summary>
/// Extracts the dishes listed on a restaurant menu photo using Gemini (Flash) vision/OCR.
/// Returns dish names (optionally with a calorie hint). Degrades gracefully: a call/parse failure
/// yields an empty, successful result so the caller can show a "no menu detected" fallback.
/// </summary>
public class GeminiMenuVisionService(
    GeminiClient client,
    ILogger<GeminiMenuVisionService> logger) : IMenuVisionService
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    private const string Prompt =
        "Read this restaurant menu image and extract each distinct dish. " +
        "Use concise English dish names. If a dish has an obvious calorie value, include it; otherwise use null. " +
        "Respond ONLY with a JSON array: [{\"dishName\": \"english name\", \"estimatedCalories\": number|null}]. " +
        "If no menu/dishes are visible, respond with [].";

    public async Task<MenuRecognitionResult> AnalyzeMenuAsync(string imageBase64OrUri, CancellationToken ct)
    {
        string raw;
        try
        {
            raw = await client.GenerateContentAsync(Prompt, imageBase64OrUri, ct);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[Gemini] Menu recognition call failed.");
            return new MenuRecognitionResult(false, []);
        }

        try
        {
            var json = StripMarkdown(raw);
            var nodes = JsonSerializer.Deserialize<List<MenuOptionDto>>(json, JsonOptions) ?? [];
            var options = nodes
                .Where(n => !string.IsNullOrWhiteSpace(n.DishName))
                .Select(n => new MenuOption(n.DishName!.Trim(), n.EstimatedCalories))
                .ToList();
            return new MenuRecognitionResult(true, options);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "[Gemini] Could not parse menu response. Raw: {Raw}", raw);
            return new MenuRecognitionResult(true, []);
        }
    }

    /// <summary>Removes a ```json ... ``` markdown fence if the model wrapped its answer.</summary>
    private static string StripMarkdown(string content)
    {
        var trimmed = content.Trim();
        if (!trimmed.StartsWith("```")) return trimmed;
        var firstNewline = trimmed.IndexOf('\n');
        var lastFence = trimmed.LastIndexOf("```", StringComparison.Ordinal);
        return firstNewline != -1 && lastFence > firstNewline
            ? trimmed[(firstNewline + 1)..lastFence].Trim()
            : trimmed;
    }

    private sealed record MenuOptionDto(string? DishName, decimal? EstimatedCalories);
}
