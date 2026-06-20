namespace Nutrisense.Nutrisense.Platform.ActivityWearable.Application.Errors;

/// <summary>Failure modes for deleting an activity log.</summary>
public enum DeleteActivityLogError
{
    /// <summary>No activity log exists with the supplied identifier.</summary>
    NotFound,

    /// <summary>The requesting user is not the owner of the activity log.</summary>
    NotOwner,

    /// <summary>An unforeseen error occurred while deleting the activity log.</summary>
    UnexpectedError
}
