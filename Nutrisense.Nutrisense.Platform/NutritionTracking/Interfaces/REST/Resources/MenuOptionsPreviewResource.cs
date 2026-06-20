namespace Nutrisense.Nutrisense.Platform.NutritionTracking.Interfaces.REST.Resources;

public record MenuOptionsPreviewResource(IEnumerable<MenuOptionResource> Options)
{
    /// <summary>List of menu options detected from the scanned menu image.</summary>
    public IEnumerable<MenuOptionResource> Options { get; } = Options;
}
