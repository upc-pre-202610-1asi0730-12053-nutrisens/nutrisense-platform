using Nutrisense.Nutrisense.Platform.ActivityWearable.Domain.Model.Aggregates;
using Nutrisense.Nutrisense.Platform.ActivityWearable.Domain.Model.Commands;
using Nutrisense.Nutrisense.Platform.ActivityWearable.Domain.Model.Errors;
using Nutrisense.Nutrisense.Platform.Shared.Application.Patterns;

namespace Nutrisense.Nutrisense.Platform.ActivityWearable.Application.CommandServices;

/// <summary>Contract for handling activity-log write operations.</summary>
public interface IActivityLogCommandService
{
    /// <summary>Logs a manual activity and triggers the caloric balance recalculation chain.</summary>
    /// <param name="command">The command carrying the activity details.</param>
    /// <param name="ct">Token to observe for cancellation.</param>
    /// <returns>The created <see cref="ActivityLog"/> on success, or a <see cref="ActivityWearableError"/> on failure.</returns>
    Task<Result<ActivityLog, ActivityWearableError>> Handle(LogManualActivityCommand command, CancellationToken ct = default);

    /// <summary>Deletes an activity log on behalf of its owner and recalculates that day's caloric balance.</summary>
    /// <param name="command">The command identifying the activity log and requesting user.</param>
    /// <param name="ct">Token to observe for cancellation.</param>
    /// <returns>True on success, or a <see cref="ActivityWearableError"/> on failure.</returns>
    Task<Result<bool, ActivityWearableError>> Handle(DeleteActivityLogCommand command, CancellationToken ct = default);
}
