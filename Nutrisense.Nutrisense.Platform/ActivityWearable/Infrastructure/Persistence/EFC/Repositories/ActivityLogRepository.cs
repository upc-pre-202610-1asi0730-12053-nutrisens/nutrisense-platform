using Microsoft.EntityFrameworkCore;
using Nutrisense.Nutrisense.Platform.ActivityWearable.Domain.Model.Aggregates;
using Nutrisense.Nutrisense.Platform.ActivityWearable.Domain.Repositories;
using Nutrisense.Nutrisense.Platform.Shared.Domain.Repositories;
using Nutrisense.Nutrisense.Platform.Shared.Infrastructure.Persistence.EFC.Configuration;
using Nutrisense.Nutrisense.Platform.Shared.Infrastructure.Persistence.EFC.Repositories;

namespace Nutrisense.Nutrisense.Platform.ActivityWearable.Infrastructure.Persistence.EFC.Repositories;

/// <summary>EF Core implementation of <see cref="IActivityLogRepository"/> backed by <see cref="AppDbContext"/>.</summary>
public class ActivityLogRepository(AppDbContext context)
    : BaseRepository<ActivityLog>(context), IActivityLogRepository
{
    /// <summary>Retrieves all activity logs for a user on a specific day.</summary>
    /// <param name="userId">Identifier of the user.</param>
    /// <param name="date">Calendar day to filter by.</param>
    /// <param name="ct">Token to observe for cancellation.</param>
    /// <returns>The activities the user logged on the given day.</returns>
    public async Task<IEnumerable<ActivityLog>> FindByUserAndDateAsync(int userId, DateOnly date, CancellationToken ct = default) =>
        await Context.Set<ActivityLog>()
            .Where(a => a.UserId == userId && a.Date == date)
            .ToListAsync(ct);

    /// <summary>Retrieves a user's activity logs, optionally bounded by an inclusive date range, newest first.</summary>
    /// <param name="userId">Identifier of the user.</param>
    /// <param name="from">Inclusive lower bound of the date range, or null for no lower bound.</param>
    /// <param name="to">Inclusive upper bound of the date range, or null for no upper bound.</param>
    /// <param name="ct">Token to observe for cancellation.</param>
    /// <returns>The matching activities, ordered from newest to oldest.</returns>
    public async Task<IEnumerable<ActivityLog>> FindByUserAsync(int userId, DateOnly? from, DateOnly? to, CancellationToken ct = default)
    {
        var query = Context.Set<ActivityLog>().Where(a => a.UserId == userId);
        if (from.HasValue) query = query.Where(a => a.Date >= from.Value);
        if (to.HasValue) query = query.Where(a => a.Date <= to.Value);
        return await query.OrderByDescending(a => a.Date).ToListAsync(ct);
    }

    /// <summary>Computes the total calories burned by a user on a specific day.</summary>
    /// <param name="userId">Identifier of the user.</param>
    /// <param name="date">Calendar day to aggregate over.</param>
    /// <param name="ct">Token to observe for cancellation.</param>
    /// <returns>The sum of calories burned that day, in kilocalories.</returns>
    public async Task<decimal> GetDailyCaloriesBurnedAsync(int userId, DateOnly date, CancellationToken ct = default)
    {
        var logs = await Context.Set<ActivityLog>()
            .Where(a => a.UserId == userId && a.Date == date)
            .ToListAsync(ct);
        return logs.Sum(l => l.CaloriesBurned);
    }

    Task<ActivityLog?> IBaseRepository<ActivityLog>.FindByIdAsync(int id, CancellationToken ct) =>
        base.FindByIdAsync(id, ct);
}
