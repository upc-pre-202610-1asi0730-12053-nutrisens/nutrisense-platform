using Nutrisense.Nutrisense.Platform.IAM.Domain.Model.Aggregates;
using Nutrisense.Nutrisense.Platform.IAM.Domain.Model.Entities;
using Nutrisense.Nutrisense.Platform.IAM.Domain.Model.Queries;

namespace Nutrisense.Nutrisense.Platform.IAM.Application.QueryServices;

public interface IUserQueryService
{
    Task<User?> Handle(GetUserByIdQuery query);
    Task<User?> Handle(GetUserByEmailQuery query);
    Task<IEnumerable<UserSession>> Handle(GetAllSessionsByUserIdQuery query);
    Task<IEnumerable<DietaryRestriction>> Handle(GetDietaryRestrictionsByUserIdQuery query);
}
