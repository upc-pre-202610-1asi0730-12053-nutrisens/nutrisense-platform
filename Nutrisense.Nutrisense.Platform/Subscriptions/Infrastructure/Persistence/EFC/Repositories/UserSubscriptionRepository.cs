using Microsoft.EntityFrameworkCore;
using Nutrisense.Nutrisense.Platform.Shared.Domain.Repositories;
using Nutrisense.Nutrisense.Platform.Shared.Infrastructure.Persistence.EFC.Configuration;
using Nutrisense.Nutrisense.Platform.Shared.Infrastructure.Persistence.EFC.Repositories;
using Nutrisense.Nutrisense.Platform.Subscriptions.Domain.Model.Aggregates;
using Nutrisense.Nutrisense.Platform.Subscriptions.Domain.Repositories;

namespace Nutrisense.Nutrisense.Platform.Subscriptions.Infrastructure.Persistence.EFC.Repositories;

public class UserSubscriptionRepository(AppDbContext context)
    : BaseRepository<UserSubscription>(context), IUserSubscriptionRepository
{
    private IQueryable<UserSubscription> WithRelations() =>
        Context.Set<UserSubscription>().Include(s => s.PaymentRecords);

    public new async Task<UserSubscription?> FindByIdAsync(int id, CancellationToken ct = default) =>
        await WithRelations().FirstOrDefaultAsync(s => s.Id == id, ct);

    Task<UserSubscription?> IBaseRepository<UserSubscription>.FindByIdAsync(int id, CancellationToken ct) =>
        FindByIdAsync(id, ct);

    public async Task<UserSubscription?> FindByUserIdAsync(int userId, CancellationToken ct = default) =>
        await WithRelations().FirstOrDefaultAsync(s => s.UserId == userId, ct);

    public async Task<UserSubscription?> FindActiveByUserIdAsync(int userId, CancellationToken ct = default) =>
        await WithRelations().FirstOrDefaultAsync(s => s.UserId == userId && s.Status == "active", ct);
}
