namespace Nutrisense.Nutrisense.Platform.BodyHealthMetrics.Application.Errors;

/// <summary>Failure cases for the update-weight use case.</summary>
public enum UpdateWeightError
{
    BodyMetricsNotFound,
    InvalidData,
    UnexpectedError
}
