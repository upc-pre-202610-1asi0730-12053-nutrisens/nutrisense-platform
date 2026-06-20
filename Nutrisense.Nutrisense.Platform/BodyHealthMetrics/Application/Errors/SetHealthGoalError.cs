namespace Nutrisense.Nutrisense.Platform.BodyHealthMetrics.Application.Errors;

/// <summary>Failure cases for the set-health-goal use case.</summary>
public enum SetHealthGoalError
{
    BodyMetricsNotFound,
    InvalidData,
    UnexpectedError
}
