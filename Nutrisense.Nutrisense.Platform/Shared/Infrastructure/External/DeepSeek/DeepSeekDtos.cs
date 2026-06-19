namespace Nutrisense.Nutrisense.Platform.Shared.Infrastructure.External.DeepSeek;

/// <summary>Request/response DTOs for the DeepSeek (OpenAI-compatible) chat completions API.</summary>
public sealed record DeepSeekRequest(string Model, List<DeepSeekMessage> Messages, double Temperature);

public sealed record DeepSeekMessage(string Role, string Content);

public sealed record DeepSeekResponse(List<DeepSeekChoice>? Choices);

public sealed record DeepSeekChoice(DeepSeekMessage? Message);
