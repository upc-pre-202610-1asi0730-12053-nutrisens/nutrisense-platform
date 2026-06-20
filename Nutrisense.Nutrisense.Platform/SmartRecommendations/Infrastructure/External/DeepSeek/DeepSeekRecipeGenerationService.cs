using System.Text;
using System.Text.Json;
using Nutrisense.Nutrisense.Platform.Shared.Infrastructure.External.DeepSeek;
using Nutrisense.Nutrisense.Platform.SmartRecommendations.Domain.Services;

namespace Nutrisense.Nutrisense.Platform.SmartRecommendations.Infrastructure.External.DeepSeek;

/// <summary>
/// Generates recipes for a goal via DeepSeek, constrained to the available ingredient catalog keys.
/// Returns an empty batch on any failure so an import run degrades gracefully.
/// </summary>
public class DeepSeekRecipeGenerationService(
    DeepSeekClient client,
    ILogger<DeepSeekRecipeGenerationService> logger) : IRecipeGenerationService
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    public async Task<IReadOnlyList<GeneratedRecipeData>> GenerateAsync(
        IReadOnlyDictionary<string, IReadOnlyList<string>> ingredientKeysByCategory,
        string goalType,
        int count,
        CancellationToken ct = default)
    {
        if (ingredientKeysByCategory.Count == 0 || count <= 0) return [];

        string raw;
        try
        {
            raw = await client.ChatAsync(BuildPrompt(ingredientKeysByCategory, goalType, count), 0.5, ct);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[DeepSeek] Recipe generation call failed for goal={Goal}.", goalType);
            return [];
        }

        try
        {
            var json = StripMarkdown(raw);
            var nodes = JsonSerializer.Deserialize<List<RecipeDto>>(json, JsonOptions) ?? [];
            return nodes
                .Where(n => !string.IsNullOrWhiteSpace(n.NameEn))
                .Select(n => new GeneratedRecipeData(
                    n.NameEn!.Trim(),
                    string.IsNullOrWhiteSpace(n.NameEs) ? n.NameEn!.Trim() : n.NameEs!.Trim(),
                    n.PrepTimeMinutes ?? 0,
                    n.Servings ?? 1,
                    n.Calories ?? 0, n.Protein ?? 0, n.Carbs ?? 0, n.Fat ?? 0, n.Fiber ?? 0,
                    n.RestrictionsConflict ?? [],
                    (n.Ingredients ?? [])
                        .Where(i => !string.IsNullOrWhiteSpace(i.Key))
                        .Select(i => new GeneratedRecipeIngredient(i.Key!.Trim(), i.Quantity ?? 0, i.Unit?.Trim() ?? ""))
                        .ToList()))
                .ToList();
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "[DeepSeek] Could not parse recipe response for goal={Goal}. Raw: {Raw}", goalType, raw);
            return [];
        }
    }

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

    private static string BuildPrompt(
        IReadOnlyDictionary<string, IReadOnlyList<string>> byCategory, string goalType, int count)
    {
        var catalog = new StringBuilder();
        foreach (var (category, keys) in byCategory)
            catalog.Append("- ").Append(category).Append(": ").Append(JsonSerializer.Serialize(keys)).Append('\n');

        return $$"""
            Eres un chef nutricionista. Genera EXACTAMENTE {{count}} recetas saludables para el objetivo "{{goalType}}"
            (weight-loss = bajas en calorías y altas en saciedad; muscle-gain = altas en proteína y calorías).

            Usa SOLO los siguientes ingredientes disponibles (identificados por su "key"), agrupados por categoría:
            {{catalog}}
            Para cada receta devuelve un objeto JSON con:
            - nameEn: nombre en inglés, conciso.
            - nameEs: nombre en español.
            - prepTimeMinutes: entero (minutos de preparación).
            - servings: entero (porciones).
            - calories, protein, carbs, fat, fiber: números (totales de la receta; protein/carbs/fat/fiber en gramos).
            - restrictionsConflict: array con los alérgenos que la receta CONTIENE, elegidos SOLO de
              [gluten, lactose, nuts, seafood, shellfish, egg, soy, pork]. Si no contiene ninguno, [].
            - ingredients: array de objetos { "key": <una key EXACTA de la lista disponible>, "quantity": <número>, "unit": "g"|"ml"|"unit" }.
              USA ÚNICAMENTE keys de la lista de arriba; no inventes ingredientes.

            Responde SOLO con el JSON array de {{count}} recetas, sin texto adicional.
            """;
    }

    private sealed record RecipeDto(
        string? NameEn,
        string? NameEs,
        int? PrepTimeMinutes,
        int? Servings,
        decimal? Calories,
        decimal? Protein,
        decimal? Carbs,
        decimal? Fat,
        decimal? Fiber,
        List<string>? RestrictionsConflict,
        List<RecipeIngredientDto>? Ingredients);

    private sealed record RecipeIngredientDto(string? Key, decimal? Quantity, string? Unit);
}
