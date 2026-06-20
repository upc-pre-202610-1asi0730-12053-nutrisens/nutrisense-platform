using Microsoft.EntityFrameworkCore;
using Nutrisense.Nutrisense.Platform.Shared.Domain.Repositories;
using Nutrisense.Nutrisense.Platform.Shared.Infrastructure.Persistence.EFC.Configuration;
using Nutrisense.Nutrisense.Platform.Shared.Infrastructure.Persistence.EFC.Repositories;
using Nutrisense.Nutrisense.Platform.SmartRecommendations.Domain.Model.Aggregates;
using Nutrisense.Nutrisense.Platform.SmartRecommendations.Domain.Repositories;

namespace Nutrisense.Nutrisense.Platform.SmartRecommendations.Infrastructure.Persistence.EFC.Repositories;

public class RecommendationCardRepository(AppDbContext context)
    : BaseRepository<RecommendationCard>(context), IRecommendationCardRepository
{
    public async Task<RecommendationCard?> FindActiveByUserIdAsync(int userId, CancellationToken ct = default) =>
        await Context.Set<RecommendationCard>()
            .FirstOrDefaultAsync(rc => rc.UserId == userId && rc.IsActive, ct);

    public async Task<IEnumerable<RecommendationCard>> FindActiveListByUserIdAsync(int userId, CancellationToken ct = default) =>
        await Context.Set<RecommendationCard>()
            .Where(rc => rc.UserId == userId && rc.IsActive)
            .OrderByDescending(rc => rc.CreatedAt)
            .ToListAsync(ct);

    public async Task<IEnumerable<RecommendationCard>> FindByUserIdAsync(int userId, CancellationToken ct = default) =>
        await Context.Set<RecommendationCard>()
            .Where(rc => rc.UserId == userId)
            .OrderByDescending(rc => rc.CreatedAt)
            .ToListAsync(ct);

    public async Task DeactivateAllByUserIdAsync(int userId, CancellationToken ct = default)
    {
        var active = await Context.Set<RecommendationCard>()
            .Where(rc => rc.UserId == userId && rc.IsActive)
            .ToListAsync(ct);

        foreach (var card in active)
            card.Deactivate();

        Context.Set<RecommendationCard>().UpdateRange(active);
    }

    Task<RecommendationCard?> IBaseRepository<RecommendationCard>.FindByIdAsync(int id, CancellationToken ct) =>
        base.FindByIdAsync(id, ct);
}
