using Nutrisense.Nutrisense.Platform.IAM.Application.QueryServices;
using Nutrisense.Nutrisense.Platform.IAM.Domain.Model.Queries;
using Nutrisense.Nutrisense.Platform.IAM.Domain.Model.ValueObjects;
using Nutrisense.Nutrisense.Platform.IAM.Interfaces.Acl;

namespace Nutrisense.Nutrisense.Platform.IAM.Application.Acl;

/// <inheritdoc cref="IIamContextFacade"/>
public class IamContextFacade(IUserQueryService userQueryService) : IIamContextFacade
{
    public async Task<IReadOnlyList<string>> GetDietaryRestrictionsByUserId(
        int userId, CancellationToken ct = default)
    {
        try
        {
            var query = new GetDietaryRestrictionsByUserIdQuery(new UserId(userId));
            var restrictions = await userQueryService.Handle(query);
            return restrictions.Select(r => r.Restriction).ToList();
        }
        catch
        {
            return [];
        }
    }
}
