using Nutrisense.Nutrisense.Platform.IAM.Domain.Services;

namespace Nutrisense.Nutrisense.Platform.IAM.Infrastructure.Hashing.BCrypt;

public class BCryptHashingService : IHashingService
{
    public string Hash(string plain) =>
        global::BCrypt.Net.BCrypt.HashPassword(plain);

    public bool Verify(string plain, string hash) =>
        global::BCrypt.Net.BCrypt.Verify(plain, hash);
}
