using Microsoft.EntityFrameworkCore;
using Nutrisense.Nutrisense.Platform.Shared.Domain.Repositories;
using Nutrisense.Nutrisense.Platform.Shared.Infrastructure.Persistence.EFC.Configuration;
using Nutrisense.Nutrisense.Platform.Shared.Infrastructure.Persistence.EFC.Repositories;
using Nutrisense.Nutrisense.Platform.SmartRecommendations.Domain.Model.Aggregates;
using Nutrisense.Nutrisense.Platform.SmartRecommendations.Domain.Repositories;

namespace Nutrisense.Nutrisense.Platform.SmartRecommendations.Infrastructure.Persistence.EFC.Repositories;

public class PantryRepository(AppDbContext context)
    : BaseRepository<Pantry>(context), IPantryRepository
{
    private IQueryable<Pantry> WithItems() =>
        Context.Set<Pantry>().Include(p => p.Items);

    public async Task<Pantry?> FindByUserIdAsync(int userId, CancellationToken ct = default) =>
        await WithItems().FirstOrDefaultAsync(p => p.UserId == userId, ct);

    public async Task<Pantry> FindOrCreateAsync(int userId, CancellationToken ct = default)
    {
        var existing = await FindByUserIdAsync(userId, ct);
        if (existing is not null) return existing;

        var created = new Pantry(userId);
        await Context.Set<Pantry>().AddAsync(created, ct);
        await Context.SaveChangesAsync(ct);
        return created;
    }

    Task<Pantry?> IBaseRepository<Pantry>.FindByIdAsync(int id, CancellationToken ct) =>
        base.FindByIdAsync(id, ct);
}
