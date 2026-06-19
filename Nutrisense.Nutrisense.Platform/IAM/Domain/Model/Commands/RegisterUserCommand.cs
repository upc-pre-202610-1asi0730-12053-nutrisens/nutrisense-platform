namespace Nutrisense.Nutrisense.Platform.IAM.Domain.Model.Commands;

public record RegisterUserCommand(
    string Email,
    string Password,
    string FirstName,
    string LastName,
    string PreferredLanguage);
