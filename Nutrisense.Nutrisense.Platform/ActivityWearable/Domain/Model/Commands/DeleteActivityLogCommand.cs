namespace Nutrisense.Nutrisense.Platform.ActivityWearable.Domain.Model.Commands;

/// <summary>Represents the intention to delete an activity log entry on behalf of its owner.</summary>
/// <param name="ActivityLogId">Identifier of the activity log to delete.</param>
/// <param name="UserId">Identifier of the user requesting deletion, used to enforce ownership.</param>
public record DeleteActivityLogCommand(int ActivityLogId, int UserId);
