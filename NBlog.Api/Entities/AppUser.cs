using Microsoft.AspNetCore.Identity;

namespace NBlog.Api.Entities;

public class AppUser : IdentityUser<string>
{
    public List<AppUser> Followers { get; set; } = new();
    public List<AppUser> Following { get; set; } = new();
}

public enum FollowAction
{
    Follow,
    Unfollow
}