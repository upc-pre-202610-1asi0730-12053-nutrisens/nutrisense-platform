using Microsoft.EntityFrameworkCore;
using Nutrisense.Nutrisense.Platform.ActivityWearable.Domain.Model.Aggregates;
using Nutrisense.Nutrisense.Platform.ActivityWearable.Domain.Repositories;
using Nutrisense.Nutrisense.Platform.Shared.Domain.Repositories;
using Nutrisense.Nutrisense.Platform.Shared.Infrastructure.Persistence.EFC.Configuration;
using Nutrisense.Nutrisense.Platform.Shared.Infrastructure.Persistence.EFC.Repositories;

namespace Nutrisense.Nutrisense.Platform.ActivityWearable.Infrastructure.Persistence.EFC.Repositories;

/// <summary>EF Core implementation of <see cref="IWearableConnectionRepository"/> backed by <see cref="AppDbContext"/>.</summary>
public class WearableConnectionRepository(AppDbContext context)
    : BaseRepository<WearableConnection>(context), IWearableConnectionRepository
{
    /// <summary>Retrieves all wearable connections belonging to a user.</summary>
    /// <param name="userId">Identifier of the user.</param>
    /// <param name="ct">Token to observe for cancellation.</param>
    /// <returns>The user's wearable connections across all providers and states.</returns>
    public async Task<IEnumerable<WearableConnection>> FindByUserIdAsync(int userId, CancellationToken ct = default) =>
        await Context.Set<WearableConnection>()
            .Where(w => w.UserId == userId)
            .ToListAsync(ct);

    /// <summary>Retrieves the user's active ("connected") connection for a given provider, if any.</summary>
    /// <param name="userId">Identifier of the user.</param>
    /// <param name="provider">External provider to match (e.g. "google-fit").</param>
    /// <param name="ct">Token to observe for cancellation.</param>
    /// <returns>The matching connected connection, or null if none exists.</returns>
    public async Task<WearableConnection?> FindByUserAndProviderAsync(int userId, string provider, CancellationToken ct = default) =>
        await Context.Set<WearableConnection>()
            .Where(w => w.UserId == userId && w.Provider == provider && w.Status == "connected")
            .FirstOrDefaultAsync(ct);

    Task<WearableConnection?> IBaseRepository<WearableConnection>.FindByIdAsync(int id, CancellationToken ct) =>
        base.FindByIdAsync(id, ct);
}
