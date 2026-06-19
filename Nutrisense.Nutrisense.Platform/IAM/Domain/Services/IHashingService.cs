namespace Nutrisense.Nutrisense.Platform.IAM.Domain.Services;

public interface IHashingService
{
    string Hash(string plain);
    bool Verify(string plain, string hash);
}
