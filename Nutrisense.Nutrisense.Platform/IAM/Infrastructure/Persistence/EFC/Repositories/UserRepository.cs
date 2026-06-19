using Microsoft.EntityFrameworkCore;
using Nutrisense.Nutrisense.Platform.IAM.Domain.Model.Aggregates;
using Nutrisense.Nutrisense.Platform.IAM.Domain.Model.ValueObjects;
using Nutrisense.Nutrisense.Platform.IAM.Domain.Repositories;
using Nutrisense.Nutrisense.Platform.Shared.Domain.Repositories;
using Nutrisense.Nutrisense.Platform.Shared.Infrastructure.Persistence.EFC.Configuration;
using Nutrisense.Nutrisense.Platform.Shared.Infrastructure.Persistence.EFC.Repositories;

namespace Nutrisense.Nutrisense.Platform.IAM.Infrastructure.Persistence.EFC.Repositories;

public class UserRepository(AppDbContext context) : BaseRepository<User>(context), IUserRepository
{
    private IQueryable<User> WithRelations() =>
        Context.Set<User>()
            .Include(u => u.Sessions)
            .Include(u => u.DietaryRestrictions);

    public new async Task<User?> FindByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var userId = new UserId(id);
        return await WithRelations().FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
    }

    // Route the interface call through the shadowing method above
    Task<User?> IBaseRepository<User>.FindByIdAsync(int id, CancellationToken cancellationToken) =>
        FindByIdAsync(id, cancellationToken);

    public async Task<User?> FindByEmailAsync(Email email, CancellationToken ct = default) =>
        await WithRelations().FirstOrDefaultAsync(u => u.Email == email, ct);

    public async Task<bool> ExistsByEmailAsync(Email email, CancellationToken ct = default) =>
        await Context.Set<User>().AnyAsync(u => u.Email == email, ct);
}
