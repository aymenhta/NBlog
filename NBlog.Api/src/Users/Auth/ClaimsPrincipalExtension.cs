using System.Security.Claims;

namespace NBlog.Api.Users;

public static class ClaimsPrincipalExtension
{
    public static string GetCurrentUserId(this ClaimsPrincipal user)
    {
        return user.Claims.First(i => i.Type == "uid").Value;
    }
}