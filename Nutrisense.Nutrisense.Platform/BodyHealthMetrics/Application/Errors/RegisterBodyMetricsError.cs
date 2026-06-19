namespace Nutrisense.Nutrisense.Platform.BodyHealthMetrics.Application.Errors;

/// <summary>Failure cases for the register-body-metrics use case.</summary>
public enum RegisterBodyMetricsError
{
    AlreadyExists,
    InvalidData,
    UnexpectedError
}
