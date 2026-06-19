using Nutrisense.Nutrisense.Platform.IAM.Domain.Model.Aggregates;
using Nutrisense.Nutrisense.Platform.IAM.Domain.Model.ValueObjects;
using Nutrisense.Nutrisense.Platform.Shared.Domain.Repositories;

namespace Nutrisense.Nutrisense.Platform.IAM.Domain.Repositories;

public interface IUserRepository : IBaseRepository<User>
{
    Task<User?> FindByEmailAsync(Email email, CancellationToken ct = default);
    Task<bool> ExistsByEmailAsync(Email email, CancellationToken ct = default);
}
