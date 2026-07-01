using Nutrisense.Nutrisense.Platform.IAM.Domain.Model.Aggregates;
using Nutrisense.Nutrisense.Platform.Shared.Domain.Repositories;

namespace Nutrisense.Nutrisense.Platform.IAM.Domain.Repositories;

public interface IPasswordResetTokenRepository : IBaseRepository<PasswordResetToken>
{
    Task<PasswordResetToken?> FindByTokenAsync(string token, CancellationToken ct = default);
    Task DeleteByUserIdAsync(int userId, CancellationToken ct = default);
}
