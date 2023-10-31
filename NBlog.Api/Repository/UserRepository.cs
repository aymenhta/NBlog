using Microsoft.AspNetCore.Identity;
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

    public async Task<FollowAction> Follow(FollowAction action, string userName, string otherUserName)
    {
        var user = await GetByName(userName);
        var otherUser = await GetByName(otherUserName);
        IdentityResult result;
        switch (action)
        {
            case FollowAction.Follow:
                user.Following.Add(otherUser);
                result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                    throw new InvalidOperationException(
                        $"user '{user.UserName}' could not follow user '{otherUser.UserName}'");
                break;
            case FollowAction.Unfollow:
                user.Following.Remove(otherUser);
                result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                    throw new InvalidOperationException(
                        $"user '{user.UserName}' could not unfollow user '{otherUser.UserName}'");
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(action), action, null);
        }

        return action;
    }
}