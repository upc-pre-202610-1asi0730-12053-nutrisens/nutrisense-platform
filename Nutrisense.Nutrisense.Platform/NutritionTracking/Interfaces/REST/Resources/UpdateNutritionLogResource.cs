namespace Nutrisense.Nutrisense.Platform.NutritionTracking.Interfaces.REST.Resources;

public record UpdateNutritionLogResource(int UserId, decimal QuantityG)
{
    /// <summary>User ID of the person updating the log entry.</summary>
    public int UserId { get; } = UserId;

    /// <summary>Updated quantity of food in grams (g).</summary>
    public decimal QuantityG { get; } = QuantityG;
}
