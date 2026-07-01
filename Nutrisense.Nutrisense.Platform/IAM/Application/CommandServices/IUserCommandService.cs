using Nutrisense.Nutrisense.Platform.IAM.Application.Internal;
using Nutrisense.Nutrisense.Platform.IAM.Domain.Model.Aggregates;
using Nutrisense.Nutrisense.Platform.IAM.Domain.Model.Commands;
using Nutrisense.Nutrisense.Platform.IAM.Domain.Model.Errors;
using Nutrisense.Nutrisense.Platform.Shared.Application.Patterns;

namespace Nutrisense.Nutrisense.Platform.IAM.Application.CommandServices;

public interface IUserCommandService
{
    Task<Result<User, IamError>> Handle(RegisterUserCommand command);
    Task<Result<LoginResult, IamError>> Handle(LoginUserCommand command);
    Task<Result<bool, IamError>> Handle(LogoutUserCommand command);
    Task<Result<User, IamError>> Handle(UpdateProfileCommand command);
    Task<Result<User, IamError>> Handle(SetHealthGoalCommand command);
    Task<Result<User, IamError>> Handle(SetDietaryRestrictionsCommand command);
    Task<Result<bool, IamError>> Handle(DeleteUserCommand command);
    Task<Result<bool, IamError>> Handle(RequestPasswordResetCommand command);
    Task<Result<bool, IamError>> Handle(ResetPasswordCommand command);
}
