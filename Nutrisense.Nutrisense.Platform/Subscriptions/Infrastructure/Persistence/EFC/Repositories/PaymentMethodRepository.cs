using Microsoft.EntityFrameworkCore;
using Nutrisense.Nutrisense.Platform.Shared.Infrastructure.Persistence.EFC.Configuration;
using Nutrisense.Nutrisense.Platform.Shared.Infrastructure.Persistence.EFC.Repositories;
using Nutrisense.Nutrisense.Platform.Subscriptions.Domain.Model.Aggregates;
using Nutrisense.Nutrisense.Platform.Subscriptions.Domain.Repositories;

namespace Nutrisense.Nutrisense.Platform.Subscriptions.Infrastructure.Persistence.EFC.Repositories;

public class PaymentMethodRepository(AppDbContext context)
    : BaseRepository<PaymentMethod>(context), IPaymentMethodRepository
{
    public async Task<IEnumerable<PaymentMethod>> FindByUserIdAsync(int userId, CancellationToken ct = default) =>
        await Context.Set<PaymentMethod>().Where(m => m.UserId == userId).ToListAsync(ct);
}
