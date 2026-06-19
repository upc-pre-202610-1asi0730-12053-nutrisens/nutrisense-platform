using Nutrisense.Nutrisense.Platform.IAM.Domain.Model.Aggregates;

namespace Nutrisense.Nutrisense.Platform.IAM.Domain.Services;

public interface ITokenService
{
    string Generate(User user, int sessionId);
    int? ValidateAndGetUserId(string token);
}
