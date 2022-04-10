namespace  Helpers;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

public static class PrincipalUtilities
{
    public static int GetId(this ClaimsPrincipal principal)
    {
        var input = principal.FindFirstValue(JwtRegisteredClaimNames.Sub);
        
        if (string.IsNullOrEmpty(input))
        {
           input=principal.FindFirstValue(ClaimTypes.NameIdentifier);
        }
            int.TryParse(input, out var userId);
            return userId;


    }
}