using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Nutrisense.Nutrisense.Platform.Shared.Infrastructure.External.DeepSeek;

/// <summary>
/// Thin client over the DeepSeek (OpenAI-compatible) chat completions endpoint. Shared across
/// bounded contexts (food enrichment, recipe generation). Returns the raw assistant message content.
/// </summary>
public class DeepSeekClient(HttpClient httpClient, IConfiguration configuration)
{
    public async Task<string> ChatAsync(string prompt, double temperature, CancellationToken ct = default)
    {
        var apiKey = configuration["DeepSeek:ApiKey"];
        if (string.IsNullOrWhiteSpace(apiKey))
            throw new InvalidOperationException("DeepSeek API key not configured (DeepSeek:ApiKey).");

        var baseUrl = configuration["DeepSeek:BaseUrl"]?.TrimEnd('/') ?? "https://api.deepseek.com";
        var model = configuration["DeepSeek:Model"] ?? "deepseek-chat";

        var request = new HttpRequestMessage(HttpMethod.Post, $"{baseUrl}/chat/completions")
        {
            Content = JsonContent.Create(new DeepSeekRequest(
                model,
                [new DeepSeekMessage("user", prompt)],
                temperature))
        };
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

        var response = await httpClient.SendAsync(request, ct);
        response.EnsureSuccessStatusCode();

        var payload = await response.Content.ReadFromJsonAsync<DeepSeekResponse>(ct);
        return payload?.Choices?.FirstOrDefault()?.Message?.Content ?? "[]";
    }
}
