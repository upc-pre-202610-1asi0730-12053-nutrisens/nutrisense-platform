namespace Nutrisense.Nutrisense.Platform.BodyHealthMetrics.Interfaces.REST.Resources;

/// <summary>API response carrying a user's current BMI value and WHO weight-status category.</summary>
public record BmiResource(
    /// <summary>Body Mass Index value.</summary>
    decimal Value,
    /// <summary>WHO BMI category: Underweight, Normal, Overweight, Obese.</summary>
    string Category);
