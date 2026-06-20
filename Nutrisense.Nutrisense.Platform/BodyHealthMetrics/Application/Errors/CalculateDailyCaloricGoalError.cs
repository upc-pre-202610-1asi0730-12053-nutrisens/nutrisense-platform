namespace Nutrisense.Nutrisense.Platform.BodyHealthMetrics.Application.Errors;

/// <summary>Failure cases for the calculate-daily-caloric-goal use case.</summary>
public enum CalculateDailyCaloricGoalError
{
    BodyMetricsNotFound,
    NoActiveGoal,
    TdeeNotCalculated,
    UnexpectedError
}
