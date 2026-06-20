using Nutrisense.Nutrisense.Platform.ActivityWearable.Domain.Model.Aggregates;
using Nutrisense.Nutrisense.Platform.Shared.Domain.Repositories;

namespace Nutrisense.Nutrisense.Platform.ActivityWearable.Domain.Repositories;

/// <summary>Repository of <see cref="ActivityLog"/> aggregates.</summary>
public interface IActivityLogRepository : IBaseRepository<ActivityLog>
{
    /// <summary>Retrieves all activity logs for a user on a specific day.</summary>
    /// <param name="userId">Identifier of the user.</param>
    /// <param name="date">Calendar day to filter by.</param>
    /// <param name="ct">Token to observe for cancellation.</param>
    /// <returns>The activities the user logged on the given day.</returns>
    Task<IEnumerable<ActivityLog>> FindByUserAndDateAsync(int userId, DateOnly date, CancellationToken ct = default);

    /// <summary>Retrieves a user's activity logs, optionally bounded by an inclusive date range.</summary>
    /// <param name="userId">Identifier of the user.</param>
    /// <param name="from">Inclusive lower bound of the date range, or null for no lower bound.</param>
    /// <param name="to">Inclusive upper bound of the date range, or null for no upper bound.</param>
    /// <param name="ct">Token to observe for cancellation.</param>
    /// <returns>The matching activities, ordered from newest to oldest.</returns>
    Task<IEnumerable<ActivityLog>> FindByUserAsync(int userId, DateOnly? from, DateOnly? to, CancellationToken ct = default);

    /// <summary>Computes the total calories burned by a user on a specific day.</summary>
    /// <param name="userId">Identifier of the user.</param>
    /// <param name="date">Calendar day to aggregate over.</param>
    /// <param name="ct">Token to observe for cancellation.</param>
    /// <returns>The sum of calories burned that day, in kilocalories.</returns>
    Task<decimal> GetDailyCaloriesBurnedAsync(int userId, DateOnly date, CancellationToken ct = default);
}
