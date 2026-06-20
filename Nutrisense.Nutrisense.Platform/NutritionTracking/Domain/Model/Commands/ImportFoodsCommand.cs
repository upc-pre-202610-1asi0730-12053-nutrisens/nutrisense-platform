namespace Nutrisense.Nutrisense.Platform.NutritionTracking.Domain.Model.Commands;

/// <summary>Imports foods matching <paramref name="Query"/> from the external catalog into the local cache.</summary>
public record ImportFoodsCommand(string Query, int MaxResults, string DataType);
