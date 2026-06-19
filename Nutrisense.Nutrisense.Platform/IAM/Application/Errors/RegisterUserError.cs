namespace Nutrisense.Nutrisense.Platform.IAM.Application.Errors;

public enum RegisterUserError
{
    EmailAlreadyTaken,
    WeakPassword,
    InvalidEmail,
    UnexpectedError
}
