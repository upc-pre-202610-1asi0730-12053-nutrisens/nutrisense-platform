using System.Text.Json;
using Nutrisense.Nutrisense.Platform.NutritionTracking.Domain.Services;
using Nutrisense.Nutrisense.Platform.Shared.Infrastructure.External.Gemini;

namespace Nutrisense.Nutrisense.Platform.NutritionTracking.Infrastructure.External.Gemini;

/// <summary>
/// Recognizes the food items present in a plate photo using Gemini (Flash) vision. Returns each
/// detected item with an estimated weight in grams. Degrades gracefully: a call/parse failure
/// yields an empty, successful result so the caller can show a "no dishes detected" fallback.
/// </summary>
public class GeminiDishVisionService(
    GeminiClient client,
    ILogger<GeminiDishVisionService> logger) : IDishVisionService
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    private const string Prompt =
        "Identify each visible food item in this plate image and estimate its quantity in grams. " +
        "Use concise English names. " +
        "Respond ONLY with a JSON array: [{\"name\": \"english name\", \"estimatedQuantityG\": number}]. " +
        "If no food is visible, respond with [].";

    public async Task<DishRecognitionResult> RecognizeDishAsync(string imageBase64OrUri, CancellationToken ct)
    {
        string raw;
        try
        {
            raw = await client.GenerateContentAsync(Prompt, imageBase64OrUri, ct);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[Gemini] Plate recognition call failed.");
            return new DishRecognitionResult(false, []);
        }

        try
        {
            var json = StripMarkdown(raw);
            var nodes = JsonSerializer.Deserialize<List<DetectedDishItemDto>>(json, JsonOptions) ?? [];
            var items = nodes
                .Where(n => !string.IsNullOrWhiteSpace(n.Name))
                .Select(n => new DetectedDishItem(
                    n.Name!.Trim(),
                    n.EstimatedQuantityG is > 0 ? n.EstimatedQuantityG.Value : 100m))
                .ToList();
            return new DishRecognitionResult(true, items);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "[Gemini] Could not parse plate response. Raw: {Raw}", raw);
            return new DishRecognitionResult(true, []);
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

    private sealed record DetectedDishItemDto(string? Name, decimal? EstimatedQuantityG);
}
