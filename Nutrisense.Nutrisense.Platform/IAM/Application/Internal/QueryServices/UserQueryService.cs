using Nutrisense.Nutrisense.Platform.IAM.Application.QueryServices;
using Nutrisense.Nutrisense.Platform.IAM.Domain.Model.Aggregates;
using Nutrisense.Nutrisense.Platform.IAM.Domain.Model.Entities;
using Nutrisense.Nutrisense.Platform.IAM.Domain.Model.Queries;
using Nutrisense.Nutrisense.Platform.IAM.Domain.Model.ValueObjects;
using Nutrisense.Nutrisense.Platform.IAM.Domain.Repositories;

namespace Nutrisense.Nutrisense.Platform.IAM.Application.Internal.QueryServices;

public class UserQueryService(IUserRepository userRepository) : IUserQueryService
{
    public async Task<User?> Handle(GetUserByIdQuery query) =>
        await userRepository.FindByIdAsync(query.Id);

    public async Task<User?> Handle(GetUserByEmailQuery query)
    {
        try
        {
            var email = new Email(query.Email);
            return await userRepository.FindByEmailAsync(email);
        }
        catch (ArgumentException)
        {
            return null;
        }
    }

    public async Task<IEnumerable<UserSession>> Handle(GetAllSessionsByUserIdQuery query)
    {
        var user = await userRepository.FindByIdAsync(query.UserId);
        return user?.Sessions ?? [];
    }

    public async Task<IEnumerable<DietaryRestriction>> Handle(GetDietaryRestrictionsByUserIdQuery query)
    {
        var user = await userRepository.FindByIdAsync(query.UserId);
        return user?.DietaryRestrictions ?? [];
    }
}
