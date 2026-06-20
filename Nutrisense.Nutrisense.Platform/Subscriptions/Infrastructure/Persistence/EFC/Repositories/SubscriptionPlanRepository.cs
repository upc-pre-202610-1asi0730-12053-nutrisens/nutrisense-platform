using Microsoft.EntityFrameworkCore;
using Nutrisense.Nutrisense.Platform.Shared.Infrastructure.Persistence.EFC.Configuration;
using Nutrisense.Nutrisense.Platform.Shared.Infrastructure.Persistence.EFC.Repositories;
using Nutrisense.Nutrisense.Platform.Subscriptions.Domain.Model.Aggregates;
using Nutrisense.Nutrisense.Platform.Subscriptions.Domain.Repositories;

namespace Nutrisense.Nutrisense.Platform.Subscriptions.Infrastructure.Persistence.EFC.Repositories;

public class SubscriptionPlanRepository(AppDbContext context)
    : BaseRepository<SubscriptionPlan>(context), ISubscriptionPlanRepository
{
    public async Task<SubscriptionPlan?> FindByKeyAsync(string key, CancellationToken ct = default) =>
        await Context.Set<SubscriptionPlan>().FirstOrDefaultAsync(p => p.Key == key, ct);
}
