namespace Nutrisense.Nutrisense.Platform.BodyHealthMetrics.Interfaces.REST.Resources;

/// <summary>Full body-metrics API response including biometrics, computed health values, and active goal.</summary>
public record BodyMetricsResource(
    /// <summary>Unique user identifier.</summary>
    int UserId,
    /// <summary>Height in centimeters.</summary>
    decimal HeightCm,
    /// <summary>User's date of birth in ISO 8601 format (yyyy-MM-dd).</summary>
    string? DateOfBirth,
    /// <summary>Biological sex: 'M' or 'F'.</summary>
    string? BiologicalSex,
    /// <summary>Activity level: Sedentary, Light, Moderate, Active, VeryActive.</summary>
    string? ActivityLevel,
    /// <summary>Current weight in kilograms.</summary>
    decimal? CurrentWeightKg,
    /// <summary>Body Mass Index (BMI) value.</summary>
    decimal? BmiValue,
    /// <summary>WHO BMI category: Underweight, Normal, Overweight, Obese.</summary>
    string? BmiCategory,
    /// <summary>Basal Metabolic Rate (kcal/day).</summary>
    decimal? Bmr,
    /// <summary>Total Daily Energy Expenditure (kcal/day).</summary>
    decimal? Tdee,
    /// <summary>Active goal type: Lose, Maintain, Gain.</summary>
    string? ActiveGoal,
    /// <summary>Starting weight when goal was set, in kilograms.</summary>
    decimal? ActiveGoalStartWeightKg,
    /// <summary>Target weight for active goal, in kilograms.</summary>
    decimal? ActiveGoalTargetWeightKg,
    /// <summary>Weekly weight change rate for active goal, in kilograms.</summary>
    decimal? ActiveGoalWeeklyRateKg,
    /// <summary>Timestamp when goal was activated in ISO 8601 format.</summary>
    DateTimeOffset? ActiveGoalSetAt,
    /// <summary>Caloric adjustment for goal (negative for deficit, positive for surplus).</summary>
    decimal? ActiveGoalCaloricAdjustment,
    /// <summary>Daily caloric target in kilocalories.</summary>
    int? DailyCalories,
    /// <summary>Daily protein target in grams.</summary>
    decimal? ProteinG,
    /// <summary>Daily carbohydrate target in grams.</summary>
    decimal? CarbsG,
    /// <summary>Daily fat target in grams.</summary>
    decimal? FatG,
    /// <summary>Daily fiber target in grams.</summary>
    decimal? FiberG);
