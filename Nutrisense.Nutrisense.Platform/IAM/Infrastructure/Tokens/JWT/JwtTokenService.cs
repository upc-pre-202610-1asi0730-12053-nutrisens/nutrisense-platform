using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Nutrisense.Nutrisense.Platform.IAM.Domain.Model.Aggregates;
using Nutrisense.Nutrisense.Platform.IAM.Domain.Services;

namespace Nutrisense.Nutrisense.Platform.IAM.Infrastructure.Tokens.JWT;

public class JwtTokenService(IConfiguration configuration) : ITokenService
{
    public string Generate(User user, int sessionId)
    {
        var settings = configuration.GetSection("TokenSettings");
        var secret = settings["Secret"] ?? throw new InvalidOperationException("Token secret is not configured.");
        var issuer = settings["Issuer"] ?? "nutrisense-platform";
        var audience = settings["Audience"] ?? "nutrisense-clients";
        var expiresInMinutes = int.TryParse(settings["ExpiresInMinutes"], out var mins) ? mins : 1440;

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email.Value),
            new Claim("sid", sessionId.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expiresInMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public int? ValidateAndGetUserId(string token)
    {
        var secret = configuration.GetSection("TokenSettings")["Secret"];
        if (string.IsNullOrWhiteSpace(secret)) return null;

        var tokenHandler = new JwtSecurityTokenHandler();
        try
        {
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            }, out var validatedToken);

            var jwt = (JwtSecurityToken)validatedToken;
            var sub = jwt.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;
            return sub is not null && int.TryParse(sub, out var id) ? id : null;
        }
        catch
        {
            return null;
        }
    }
}
