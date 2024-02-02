using Microsoft.AspNetCore.Identity;

namespace NBlog.Api.Users;

public sealed class AppUser : IdentityUser<string>
{
    public List<AppUser> Followers { get; set; } = [];
    public List<AppUser> Following { get; set; } = [];
}

public enum FollowAction
{
    Follow,
    Unfollow
}