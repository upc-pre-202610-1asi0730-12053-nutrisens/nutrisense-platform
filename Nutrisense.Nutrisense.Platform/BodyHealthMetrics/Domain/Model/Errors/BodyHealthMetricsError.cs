namespace Nutrisense.Nutrisense.Platform.BodyHealthMetrics.Domain.Model.Errors;

/// <summary>Failure cases across all body-metrics use cases.</summary>
public enum BodyHealthMetricsError
{
    BodyMetricsNotFound,
    BodyMetricsAlreadyExists,
    InvalidBodyMetricsData,
    InvalidWeightValue,
    InvalidMeasurementValues,
    InvalidGoalData,
    NoActiveGoal,
    TdeeNotCalculated,
    UnexpectedError
}
