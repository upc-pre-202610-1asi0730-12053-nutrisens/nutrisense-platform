using Microsoft.EntityFrameworkCore;
using Nutrisense.Nutrisense.Platform.IAM.Domain.Model.Aggregates;
using Nutrisense.Nutrisense.Platform.IAM.Domain.Repositories;
using Nutrisense.Nutrisense.Platform.Shared.Infrastructure.Persistence.EFC.Configuration;
using Nutrisense.Nutrisense.Platform.Shared.Infrastructure.Persistence.EFC.Repositories;

namespace Nutrisense.Nutrisense.Platform.IAM.Infrastructure.Persistence.EFC.Repositories;

public class PasswordResetTokenRepository(AppDbContext context)
    : BaseRepository<PasswordResetToken>(context), IPasswordResetTokenRepository
{
    public async Task<PasswordResetToken?> FindByTokenAsync(string token, CancellationToken ct = default) =>
        await Context.Set<PasswordResetToken>().FirstOrDefaultAsync(t => t.Token == token, ct);

    public async Task DeleteByUserIdAsync(int userId, CancellationToken ct = default)
    {
        var existing = await Context.Set<PasswordResetToken>()
            .Where(t => t.UserId == userId)
            .ToListAsync(ct);
        if (existing.Count > 0)
            Context.Set<PasswordResetToken>().RemoveRange(existing);
    }
}
