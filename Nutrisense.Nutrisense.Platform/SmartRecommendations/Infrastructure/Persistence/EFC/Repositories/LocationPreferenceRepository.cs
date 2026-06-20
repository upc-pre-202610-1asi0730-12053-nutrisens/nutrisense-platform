using Microsoft.EntityFrameworkCore;
using Nutrisense.Nutrisense.Platform.Shared.Domain.Repositories;
using Nutrisense.Nutrisense.Platform.Shared.Infrastructure.Persistence.EFC.Configuration;
using Nutrisense.Nutrisense.Platform.Shared.Infrastructure.Persistence.EFC.Repositories;
using Nutrisense.Nutrisense.Platform.SmartRecommendations.Domain.Model.Aggregates;
using Nutrisense.Nutrisense.Platform.SmartRecommendations.Domain.Repositories;

namespace Nutrisense.Nutrisense.Platform.SmartRecommendations.Infrastructure.Persistence.EFC.Repositories;

public class LocationPreferenceRepository(AppDbContext context)
    : BaseRepository<LocationPreference>(context), ILocationPreferenceRepository
{
    public async Task<LocationPreference?> FindByUserIdAsync(int userId, CancellationToken ct = default) =>
        await Context.Set<LocationPreference>().FirstOrDefaultAsync(lp => lp.UserId == userId, ct);

    public async Task<LocationPreference> FindOrCreateAsync(int userId, CancellationToken ct = default)
    {
        var existing = await FindByUserIdAsync(userId, ct);
        if (existing is not null) return existing;

        var created = new LocationPreference(userId);
        await Context.Set<LocationPreference>().AddAsync(created, ct);
        await Context.SaveChangesAsync(ct);
        return created;
    }

    Task<LocationPreference?> IBaseRepository<LocationPreference>.FindByIdAsync(int id, CancellationToken ct) =>
        base.FindByIdAsync(id, ct);
}
