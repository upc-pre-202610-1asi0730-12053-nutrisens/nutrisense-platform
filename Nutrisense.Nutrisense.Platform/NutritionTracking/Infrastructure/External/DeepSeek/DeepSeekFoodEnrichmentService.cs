using System.Text.Json;
using Nutrisense.Nutrisense.Platform.NutritionTracking.Domain.Services;
using Nutrisense.Nutrisense.Platform.Shared.Infrastructure.External.DeepSeek;

namespace Nutrisense.Nutrisense.Platform.NutritionTracking.Infrastructure.External.DeepSeek;

/// <summary>
/// Enriches raw USDA food names via DeepSeek: clean English name, Spanish translation,
/// coarse category and the dietary conflict tags the food contains. Degrades gracefully to
/// raw-name defaults on any failure so an import never breaks on an LLM outage.
/// </summary>
public class DeepSeekFoodEnrichmentService(
    DeepSeekClient client,
    ILogger<DeepSeekFoodEnrichmentService> logger) : IFoodEnrichmentService
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    public async Task<IReadOnlyList<EnrichedFoodData>> EnrichBatchAsync(
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
            logger.LogError(ex, "[DeepSeek] Food enrichment call failed; falling back to raw names.");
            return Defaults(namesEn);
        }

        try
        {
            var json = StripMarkdown(raw);
            var nodes = JsonSerializer.Deserialize<List<EnrichedFoodDto>>(json, JsonOptions) ?? [];
            var result = new List<EnrichedFoodData>(namesEn.Count);
            for (var i = 0; i < namesEn.Count; i++)
            {
                var node = i < nodes.Count ? nodes[i] : null;
                var fallback = namesEn[i];
                result.Add(new EnrichedFoodData(
                    string.IsNullOrWhiteSpace(node?.NameEn) ? fallback : node!.NameEn!.Trim(),
                    string.IsNullOrWhiteSpace(node?.NameEs) ? fallback : node!.NameEs!.Trim(),
                    string.IsNullOrWhiteSpace(node?.Category) ? "Other" : node!.Category!.Trim(),
                    node?.Restrictions?.Where(r => !string.IsNullOrWhiteSpace(r)).Select(r => r.Trim().ToLowerInvariant()).ToList() ?? []));
            }
            return result;
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "[DeepSeek] Could not parse enrichment response; falling back to raw names. Raw: {Raw}", raw);
            return Defaults(namesEn);
        }
    }

    private static IReadOnlyList<EnrichedFoodData> Defaults(IReadOnlyList<string> namesEn) =>
        namesEn.Select(n => new EnrichedFoodData(n, n, "Other", [])).ToList();

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
            Dado este array de alimentos en inglés (nombres en formato USDA con comas y descripciones técnicas),
            para cada uno devuelve un objeto JSON con:
            - nameEn: nombre natural en inglés sin comas ni formato USDA, máximo 5 palabras, conciso y claro
              (ej: "Whole Milk 3.25%" en vez de "Milk, whole, 3.25% milkfat, with added vitamin D").
            - nameEs: traducción natural al español.
            - category: exactamente una de [Grain, Protein, Dairy, Vegetable, Fruit, Fat, Beverage, Other].
            - restrictions: array con los alérgenos/etiquetas dietéticas que el alimento CONTIENE, elegidos SOLO de
              [gluten, lactose, nuts, seafood, shellfish, egg, soy, pork]. Si no contiene ninguno, devuelve [].
              Ejemplos: el salmón ["seafood"]; la leche ["lactose"]; las almendras ["nuts"]; el pan de trigo ["gluten"]; el pollo crudo [].

            Mantén el MISMO orden que el array de entrada y devuelve EXACTAMENTE un objeto por alimento.
            Alimentos: {{namesJson}}

            Responde SOLO con el JSON array, sin texto adicional.
            """;
    }

    private sealed record EnrichedFoodDto(string? NameEn, string? NameEs, string? Category, List<string>? Restrictions);
}
