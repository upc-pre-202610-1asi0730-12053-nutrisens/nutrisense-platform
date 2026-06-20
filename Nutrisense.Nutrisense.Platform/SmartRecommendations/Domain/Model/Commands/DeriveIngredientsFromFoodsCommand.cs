namespace Nutrisense.Nutrisense.Platform.SmartRecommendations.Domain.Model.Commands;

/// <summary>Derives the ingredient catalog from the imported food catalog (idempotent, dedups by key).</summary>
public record DeriveIngredientsFromFoodsCommand();
