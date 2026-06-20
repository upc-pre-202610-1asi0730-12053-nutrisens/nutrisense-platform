namespace Nutrisense.Nutrisense.Platform.BodyHealthMetrics.Domain.Model.Commands;

/// <summary>Command to create a user's initial body-metrics profile, with optional onboarding goal intent.</summary>
/// <param name="Goal">
/// Optional goal intent ("weight-loss" / "muscle-gain") captured during onboarding.
/// When present, registration seeds a default active goal so the daily caloric/macro
/// targets are computed immediately — no separate goal-setting step is required.
/// </param>
/// <param name="WeeklyRateKg">Optional weekly weight-change pace; defaults per goal when null.</param>
public record RegisterBodyMetricsCommand(
    int UserId,
    decimal HeightCm,
    decimal WeightKg,
    DateOnly DateOfBirth,
    string BiologicalSex,
    string ActivityLevel,
    string? Goal = null,
    decimal? WeeklyRateKg = null);
