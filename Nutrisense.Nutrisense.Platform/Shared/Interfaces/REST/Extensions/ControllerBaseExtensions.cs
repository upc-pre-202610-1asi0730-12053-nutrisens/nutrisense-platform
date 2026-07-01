using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace Nutrisense.Nutrisense.Platform.Shared.Interfaces.REST.Extensions;

public static class ControllerBaseExtensions
{
    public static int GetAuthenticatedUserId(this ControllerBase controller)
    {
        var value = controller.User.FindFirstValue(ClaimTypes.NameIdentifier)
                 ?? controller.User.FindFirstValue(JwtRegisteredClaimNames.Sub);
        return int.Parse(value!);
    }
}
