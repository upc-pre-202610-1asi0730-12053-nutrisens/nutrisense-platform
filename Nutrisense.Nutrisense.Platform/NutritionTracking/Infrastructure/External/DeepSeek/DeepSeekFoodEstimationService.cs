using System.Text.Json;
using Nutrisense.Nutrisense.Platform.NutritionTracking.Domain.Services;
using Nutrisense.Nutrisense.Platform.Shared.Infrastructure.External.DeepSeek;

namespace Nutrisense.Nutrisense.Platform.NutritionTracking.Infrastructure.External.DeepSeek;

/// <summary>
/// Estimates complete nutritional data for foods not present in the local catalog, via DeepSeek:
/// clean English/Spanish names, category, serving info, per-100g macros and dietary conflict tags.
/// Degrades gracefully to neutral defaults per name on any failure so a smart scan never breaks.
/// </summary>
public class DeepSeekFoodEstimationService(
    DeepSeekClient client,
    ILogger<DeepSeekFoodEstimationService> logger) : IFoodNutritionEstimationService
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    public async Task<IReadOnlyList<EstimatedFoodData>> EstimateBatchAsync(
        IReadOnlyList<string> namesEn, CancellationToken ct = default)
    {
        if (namesEn.Count == 0) return [];

        string raw;
        try
        {
            raw = await client.ChatAsync(BuildPrompt(namesEn), 0.2, ct);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[DeepSeek] Food estimation call failed; falling back to defaults.");
            return Defaults(namesEn);
        }

        try
        {
            var json = StripMarkdown(raw);
            var nodes = JsonSerializer.Deserialize<List<EstimatedFoodDto>>(json, JsonOptions) ?? [];
            var result = new List<EstimatedFoodData>(namesEn.Count);
            for (var i = 0; i < namesEn.Count; i++)
            {
                var node = i < nodes.Count ? nodes[i] : null;
                result.Add(FromDto(node, namesEn[i]));
            }
            return result;
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "[DeepSeek] Could not parse estimation response; falling back to defaults. Raw: {Raw}", raw);
            return Defaults(namesEn);
        }
    }

    private static EstimatedFoodData FromDto(EstimatedFoodDto? node, string fallbackName) =>
        new(
            string.IsNullOrWhiteSpace(node?.NameEn) ? fallbackName : node!.NameEn!.Trim(),
            string.IsNullOrWhiteSpace(node?.NameEs) ? fallbackName : node!.NameEs!.Trim(),
            string.IsNullOrWhiteSpace(node?.Category) ? "Other" : node!.Category!.Trim(),
            node?.ServingSizeG is > 0 ? node.ServingSizeG!.Value : 100m,
            string.IsNullOrWhiteSpace(node?.ServingUnit) ? "g" : node!.ServingUnit!.Trim(),
            node?.CaloriesPer100g ?? 150m,
            node?.ProteinPer100g ?? 8m,
            node?.CarbsPer100g ?? 20m,
            node?.FatPer100g ?? 5m,
            node?.FiberPer100g ?? 2m,
            node?.SugarPer100g ?? 3m,
            node?.Restrictions?.Where(r => !string.IsNullOrWhiteSpace(r))
                .Select(r => r.Trim().ToLowerInvariant()).ToList() ?? []);

    private static IReadOnlyList<EstimatedFoodData> Defaults(IReadOnlyList<string> namesEn) =>
        namesEn.Select(n => FromDto(null, n)).ToList();

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

    private static string BuildPrompt(IReadOnlyList<string> names)
    {
        var namesJson = JsonSerializer.Serialize(names);
        return $$"""
            Dado este array de alimentos/platos en inglés, para cada uno estima sus datos nutricionales
            y devuelve un objeto JSON con:
            - nameEn: nombre natural y conciso en inglés (máximo 5 palabras).
            - nameEs: traducción natural al español.
            - category: exactamente una de [Grain, Protein, Dairy, Vegetable, Fruit, Fat, Beverage, Other].
            - servingSizeG: tamaño de porción típico en gramos (number).
            - servingUnit: unidad de la porción (normalmente "g").
            - caloriesPer100g, proteinPer100g, carbsPer100g, fatPer100g, fiberPer100g, sugarPer100g:
              valores nutricionales por 100g (numbers, estimaciones razonables).
            - restrictions: array con los alérgenos/etiquetas dietéticas que el alimento CONTIENE, elegidos SOLO de
              [gluten, lactose, nuts, seafood, shellfish, egg, soy, pork]. Si no contiene ninguno, devuelve [].

            Mantén el MISMO orden que el array de entrada y devuelve EXACTAMENTE un objeto por alimento.
            Alimentos: {{namesJson}}

            Responde SOLO con el JSON array, sin texto adicional.
            """;
    }

    private sealed record EstimatedFoodDto(
        string? NameEn,
        string? NameEs,
        string? Category,
        decimal? ServingSizeG,
        string? ServingUnit,
        decimal? CaloriesPer100g,
        decimal? ProteinPer100g,
        decimal? CarbsPer100g,
        decimal? FatPer100g,
        decimal? FiberPer100g,
        decimal? SugarPer100g,
        List<string>? Restrictions);
}
