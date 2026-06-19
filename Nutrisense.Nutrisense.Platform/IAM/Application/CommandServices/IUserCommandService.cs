using Nutrisense.Nutrisense.Platform.IAM.Application.Errors;
using Nutrisense.Nutrisense.Platform.IAM.Application.Internal;
using Nutrisense.Nutrisense.Platform.IAM.Domain.Model.Aggregates;
using Nutrisense.Nutrisense.Platform.IAM.Domain.Model.Commands;
using Nutrisense.Nutrisense.Platform.Shared.Application.Patterns;

namespace Nutrisense.Nutrisense.Platform.IAM.Application.CommandServices;

public interface IUserCommandService
{
    Task<Result<User, RegisterUserError>> Handle(RegisterUserCommand command);
    Task<Result<LoginResult, LoginUserError>> Handle(LoginUserCommand command);
    Task<Result<bool, LogoutUserError>> Handle(LogoutUserCommand command);
    Task<Result<User, UpdateProfileError>> Handle(UpdateProfileCommand command);
    Task<Result<User, SetHealthGoalError>> Handle(SetHealthGoalCommand command);
    Task<Result<User, SetDietaryRestrictionsError>> Handle(SetDietaryRestrictionsCommand command);
    Task<Result<bool, DeleteUserError>> Handle(DeleteUserCommand command);
}
