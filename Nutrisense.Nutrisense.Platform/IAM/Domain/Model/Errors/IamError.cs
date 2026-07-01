namespace Nutrisense.Nutrisense.Platform.IAM.Domain.Model.Errors;

public enum IamError
{
    UserNotFound,
    SessionNotFound,
    InvalidCredentials,
    EmailAlreadyTaken,
    WeakPassword,
    InvalidEmail,
    InvalidResetToken,
    ResetTokenExpired,
    InvalidGoal,
    InvalidProfileData,
    UnexpectedError
}
