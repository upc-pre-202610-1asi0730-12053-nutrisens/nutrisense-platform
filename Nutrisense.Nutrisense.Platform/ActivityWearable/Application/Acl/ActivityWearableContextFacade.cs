using Nutrisense.Nutrisense.Platform.ActivityWearable.Application.CommandServices;
using Nutrisense.Nutrisense.Platform.ActivityWearable.Application.QueryServices;
using Nutrisense.Nutrisense.Platform.ActivityWearable.Domain.Model.Queries;
using Nutrisense.Nutrisense.Platform.ActivityWearable.Interfaces.Acl;

namespace Nutrisense.Nutrisense.Platform.ActivityWearable.Application.Acl;

/// <summary>
/// ACL facade that exposes ActivityWearable data to other bounded contexts.
/// Implementation of <see cref="IActivityWearableContextFacade"/> that delegates to the activity-log
/// query service and degrades gracefully, returning a neutral value (0) on failure instead of throwing.
/// </summary>
public class ActivityWearableContextFacade(
    IActivityLogCommandService commandService,
    IActivityLogQueryService queryService) : IActivityWearableContextFacade
{
    /// <summary>Returns the total calories burned by the user on the given date, or 0 when there is no activity or an error occurs.</summary>
    /// <param name="userId">Identifier of the user whose calories are requested.</param>
    /// <param name="date">Calendar day to total calories for.</param>
    /// <param name="ct">Token to observe for cancellation.</param>
    /// <returns>The total calories burned that day, or 0 on absence of data or failure.</returns>
    public async Task<decimal> GetDailyCaloriesBurned(int userId, DateOnly date, CancellationToken ct = default)
    {
        try
        {
            var summary = await queryService.Handle(new GetDailyActivitySummaryQuery(userId, date), ct);
            return summary?.TotalCaloriesBurned ?? 0;
        }
        catch
        {
            return 0;
        }
    }
}
