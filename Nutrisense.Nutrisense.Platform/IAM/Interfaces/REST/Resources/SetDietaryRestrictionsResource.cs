namespace Nutrisense.Nutrisense.Platform.IAM.Interfaces.REST.Resources;

public record SetDietaryRestrictionsResource(string[] Restrictions)
{
    /// <summary>Array of dietary restriction codes. Valid values: Vegan, Vegetarian, GlutenFree, Halal, Kosher, Keto, Paleo, DairyFree, NutFree, SoyFree.</summary>
    public string[] Restrictions { get; init; } = Restrictions;
}
