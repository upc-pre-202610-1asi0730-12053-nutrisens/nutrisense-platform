using System.Text.Json;
using Nutrisense.Nutrisense.Platform.Shared.Infrastructure.External.DeepSeek;
using Nutrisense.Nutrisense.Platform.SmartRecommendations.Domain.Services;

namespace Nutrisense.Nutrisense.Platform.SmartRecommendations.Infrastructure.External.DeepSeek;

/// <summary>
/// Suggests locally-available foods for a city + weather + goal context via DeepSeek. Prioritizes, in
/// order, the city (what you can actually find/eat there), then the weather, then the nutrition goal.
/// Returns names only; macros are resolved downstream by the food-provisioning step. Returns an empty
/// list on any failure so a recommendation refresh degrades gracefully.
/// </summary>
public class DeepSeekLocalFoodSuggestionService(
    DeepSeekClient client,
    ILogger<DeepSeekLocalFoodSuggestionService> logger) : ILocalFoodSuggestionService
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    public async Task<IReadOnlyList<LocalFoodSuggestion>> SuggestAsync(
        string cityName,
        string country,
        string weatherCondition,
        decimal tempC,
        string weatherType,
        string goalType,
        IReadOnlyList<string> restrictions,
        int count,
        CancellationToken ct = default)
    {
        if (count <= 0) return [];

        string raw;
        try
        {
            raw = await client.ChatAsync(
                BuildPrompt(cityName, country, weatherCondition, tempC, weatherType, goalType, restrictions, count),
                0.7, ct);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[DeepSeek] Local food suggestion call failed for city={City}, goal={Goal}.",
                cityName, goalType);
            return [];
        }

        try
        {
            var json = StripMarkdown(raw);
            var nodes = JsonSerializer.Deserialize<List<SuggestionDto>>(json, JsonOptions) ?? [];
            return nodes
                .Where(n => !string.IsNullOrWhiteSpace(n.NameEn))
                .Select(n => new LocalFoodSuggestion(
                    n.NameEn!.Trim(),
                    string.IsNullOrWhiteSpace(n.NameEs) ? n.NameEn!.Trim() : n.NameEs!.Trim(),
                    string.IsNullOrWhiteSpace(n.Note) ? null : n.Note!.Trim()))
                .ToList();
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "[DeepSeek] Could not parse local food suggestions for city={City}. Raw: {Raw}",
                cityName, raw);
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
        string cityName, string country, string weatherCondition, decimal tempC,
        string weatherType, string goalType, IReadOnlyList<string> restrictions, int count)
    {
        var restrictionsText = restrictions.Count > 0
            ? JsonSerializer.Serialize(restrictions)
            : "[]";

        return $$"""
            Eres un nutricionista local. Sugiere EXACTAMENTE {{count}} alimentos o platos para una persona
            que está en {{cityName}} ({{country}}). Prioriza, EN ESTE ORDEN:
            1) DISPONIBILIDAD LOCAL: comida típica o fácil de encontrar y comprar en {{cityName}}.
            2) CLIMA ACTUAL: {{weatherCondition}}, {{tempC}}°C ({{weatherType}}). Ajusta a algo apetecible para ese clima
               (p.ej. con calor opciones frescas/ligeras; con frío opciones cálidas/reconfortantes).
            3) OBJETIVO: "{{goalType}}" (weight-loss = bajo en calorías y saciante; muscle-gain = alto en proteína).

            NO sugieras nada que contenga estos alérgenos/etiquetas del usuario: {{restrictionsText}}.

            Para cada sugerencia devuelve un objeto JSON con:
            - nameEn: nombre natural y conciso en inglés (máximo 5 palabras).
            - nameEs: nombre natural en español.
            - note: frase corta (máx 12 palabras) de por qué encaja con la ciudad/clima/objetivo.

            Responde SOLO con el JSON array de {{count}} objetos, sin texto adicional.
            """;
    }

    private sealed record SuggestionDto(string? NameEn, string? NameEs, string? Note);
}
