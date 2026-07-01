using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Nutrisense.Nutrisense.Platform.IAM.Application.Internal;
using Nutrisense.Nutrisense.Platform.IAM.Domain.Model.Aggregates;
using Nutrisense.Nutrisense.Platform.IAM.Domain.Model.Errors;
using Nutrisense.Nutrisense.Platform.IAM.Interfaces.REST.Resources;
using Nutrisense.Nutrisense.Platform.IAM.Resources;
using Nutrisense.Nutrisense.Platform.Shared.Application.Patterns;
using ProblemDetailsFactory = Nutrisense.Nutrisense.Platform.Shared.Interfaces.REST.ProblemDetails.ProblemDetailsFactory;

namespace Nutrisense.Nutrisense.Platform.IAM.Interfaces.REST.Transform;

/// <summary>Converts IAM command results into HTTP action results, centralizing the IamError -> HTTP status mapping.</summary>
public static class IamActionResultAssembler
{
    public static IActionResult ToRegisterResult(Result<User, IamError> result, IStringLocalizer<IAMMessages> localizer) =>
        result switch
        {
            Result<User, IamError>.Success s =>
                new ObjectResult(UserResourceAssembler.ToResource(s.Value)) { StatusCode = StatusCodes.Status201Created },
            Result<User, IamError>.Failure f => FailureResult(f.Error, localizer),
            _ => FailureResult(IamError.UnexpectedError, localizer)
        };

    public static IActionResult ToLoginResult(Result<LoginResult, IamError> result, IStringLocalizer<IAMMessages> localizer) =>
        result switch
        {
            Result<LoginResult, IamError>.Success s =>
                new OkObjectResult(new LoginResponseResource(s.Value.UserId, s.Value.Token, s.Value.SessionId)),
            Result<LoginResult, IamError>.Failure f => FailureResult(f.Error, localizer),
            _ => FailureResult(IamError.UnexpectedError, localizer)
        };

    public static IActionResult ToLogoutResult(Result<bool, IamError> result, IStringLocalizer<IAMMessages> localizer) =>
        result switch
        {
            Result<bool, IamError>.Success => new NoContentResult(),
            Result<bool, IamError>.Failure f => FailureResult(f.Error, localizer),
            _ => FailureResult(IamError.UnexpectedError, localizer)
        };

    public static IActionResult ToDeleteUserResult(Result<bool, IamError> result, IStringLocalizer<IAMMessages> localizer) =>
        result switch
        {
            Result<bool, IamError>.Success => new NoContentResult(),
            Result<bool, IamError>.Failure f => FailureResult(f.Error, localizer),
            _ => FailureResult(IamError.UnexpectedError, localizer)
        };

    public static IActionResult ToResetPasswordResult(Result<bool, IamError> result, IStringLocalizer<IAMMessages> localizer) =>
        result switch
        {
            Result<bool, IamError>.Success => new OkObjectResult(new { message = localizer["PasswordResetSuccess"].Value }),
            Result<bool, IamError>.Failure f => FailureResult(f.Error, localizer),
            _ => FailureResult(IamError.UnexpectedError, localizer)
        };

    public static IActionResult ToUpdateProfileResult(Result<User, IamError> result, IStringLocalizer<IAMMessages> localizer) =>
        result switch
        {
            Result<User, IamError>.Success s => new OkObjectResult(UserResourceAssembler.ToResource(s.Value)),
            Result<User, IamError>.Failure f => FailureResult(f.Error, localizer),
            _ => FailureResult(IamError.UnexpectedError, localizer)
        };

    public static IActionResult ToSetHealthGoalResult(Result<User, IamError> result, IStringLocalizer<IAMMessages> localizer) =>
        result switch
        {
            Result<User, IamError>.Success s => new OkObjectResult(UserResourceAssembler.ToResource(s.Value)),
            Result<User, IamError>.Failure f => FailureResult(f.Error, localizer),
            _ => FailureResult(IamError.UnexpectedError, localizer)
        };

    public static IActionResult ToSetDietaryRestrictionsResult(Result<User, IamError> result, IStringLocalizer<IAMMessages> localizer) =>
        result switch
        {
            Result<User, IamError>.Success s => new OkObjectResult(UserResourceAssembler.ToResource(s.Value)),
            Result<User, IamError>.Failure f => FailureResult(f.Error, localizer),
            _ => FailureResult(IamError.UnexpectedError, localizer)
        };

    private static ObjectResult FailureResult(IamError error, IStringLocalizer<IAMMessages> localizer) => error switch
    {
        IamError.UserNotFound => Problem(StatusCodes.Status404NotFound, localizer["NotFoundTitle"].Value, localizer["UserNotFound"].Value),
        IamError.SessionNotFound => Problem(StatusCodes.Status404NotFound, localizer["NotFoundTitle"].Value, localizer["SessionNotFound"].Value),
        IamError.InvalidCredentials => Problem(StatusCodes.Status401Unauthorized, localizer["UnauthorizedTitle"].Value, localizer["InvalidCredentials"].Value),
        IamError.EmailAlreadyTaken => Problem(StatusCodes.Status409Conflict, localizer["ConflictTitle"].Value, localizer["EmailAlreadyTaken"].Value),
        IamError.WeakPassword => Problem(StatusCodes.Status400BadRequest, localizer["BadRequestTitle"].Value, localizer["WeakPassword"].Value),
        IamError.InvalidEmail => Problem(StatusCodes.Status400BadRequest, localizer["BadRequestTitle"].Value, localizer["InvalidEmail"].Value),
        IamError.InvalidResetToken => Problem(StatusCodes.Status400BadRequest, localizer["BadRequestTitle"].Value, localizer["InvalidResetToken"].Value),
        IamError.ResetTokenExpired => Problem(StatusCodes.Status400BadRequest, localizer["BadRequestTitle"].Value, localizer["ResetTokenExpired"].Value),
        IamError.InvalidGoal => Problem(StatusCodes.Status400BadRequest, localizer["BadRequestTitle"].Value, localizer["InvalidGoal"].Value),
        IamError.InvalidProfileData => Problem(StatusCodes.Status400BadRequest, localizer["BadRequestTitle"].Value, localizer["InvalidProfileData"].Value),
        _ => Problem(StatusCodes.Status500InternalServerError, localizer["UnexpectedServerError"].Value, localizer["UnexpectedError"].Value)
    };

    private static ObjectResult Problem(int status, string title, string detail) =>
        new(ProblemDetailsFactory.Create(status, title, detail)) { StatusCode = status };
}
