using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NBlog.Api.Exceptions;

namespace NBlog.Api.Users;

public class UserRepository(UserManager<AppUser> userManager) : IUserRepository
{

    public async Task<AppUser> GetByName(string name)
        => await userManager.FindByNameAsync(name) ?? throw new ResourceNotFoundException("user could not be found");

    public async Task<AppUser> GetByEmail(string email)
        => await userManager.FindByEmailAsync(email) ?? throw new ResourceNotFoundException("user could not be found");

    public async Task<AppUser> GetById(string id)
        => await userManager.FindByIdAsync(id) ?? throw new ResourceNotFoundException("user could not be found");

    public async Task<FollowAction> Follow(string userName, string otherUserName)
    {
        var user = await GetByNameWithFollowing(userName);
        var otherUser = await GetByNameWithFollowing(otherUserName);
        IdentityResult result;
        if (!user.Following.Contains(otherUser))
        {
            user.Following.Add(otherUser);
            result = await userManager.UpdateAsync(user);
            if (!result.Succeeded)
                throw new InvalidOperationException(
                    $"user '{user.UserName}' could not follow user '{otherUser.UserName}'");
            return FollowAction.Follow;
        }

        user.Following.Remove(otherUser);
        result = await userManager.UpdateAsync(user);
        if (!result.Succeeded)
            throw new InvalidOperationException(
                $"user '{user.UserName}' could not unfollow user '{otherUser.UserName}'");
        return FollowAction.Unfollow;
    }

    private async Task<AppUser> GetByNameWithFollowing(string username)
        => await userManager.Users
               .Include(u => u.Following)
               .FirstOrDefaultAsync(u => u.UserName!.Equals(username))
           ?? throw new ResourceNotFoundException("user could not be found");
}