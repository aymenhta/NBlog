using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NBlog.Api.Entities;
using NBlog.Api.Exceptions;

namespace NBlog.Api.Repository;

public class UserRepository : IUserRepository
{
    private readonly UserManager<AppUser> _userManager;

    public UserRepository(UserManager<AppUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<AppUser> GetByName(string name)
        => await _userManager.FindByNameAsync(name) ?? throw new ResourceNotFoundException("user could not be found");

    public async Task<AppUser> GetByEmail(string email)
        => await _userManager.FindByEmailAsync(email) ?? throw new ResourceNotFoundException("user could not be found");

    public async Task<AppUser> GetById(string id)
        => await _userManager.FindByIdAsync(id) ?? throw new ResourceNotFoundException("user could not be found");

    public async Task<FollowAction> Follow(string userName, string otherUserName)
    {
        var user = await GetByNameWithFollowing(userName);
        var otherUser = await GetByNameWithFollowing(otherUserName);
        IdentityResult result;
        if (!user.Following.Contains(otherUser))
        {
            user.Following.Add(otherUser);
            result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                throw new InvalidOperationException(
                    $"user '{user.UserName}' could not follow user '{otherUser.UserName}'");
            return FollowAction.Follow;
        }

        user.Following.Remove(otherUser);
        result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
            throw new InvalidOperationException(
                $"user '{user.UserName}' could not unfollow user '{otherUser.UserName}'");
        return FollowAction.Unfollow;
    }

    private async Task<AppUser> GetByNameWithFollowing(string username)
        => await _userManager.Users
               .Include(u => u.Following)
               .FirstOrDefaultAsync(u => u.UserName!.Equals(username))
           ?? throw new ResourceNotFoundException("user could not be found");
}