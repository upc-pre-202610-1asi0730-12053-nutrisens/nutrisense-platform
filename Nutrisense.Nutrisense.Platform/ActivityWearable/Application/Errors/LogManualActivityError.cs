namespace Nutrisense.Nutrisense.Platform.ActivityWearable.Application.Errors;

/// <summary>Failure modes for logging a manual activity.</summary>
public enum LogManualActivityError
{
    /// <summary>The submitted activity data failed domain validation.</summary>
    InvalidActivity,

    /// <summary>An unforeseen error occurred while logging the activity.</summary>
    UnexpectedError
}
