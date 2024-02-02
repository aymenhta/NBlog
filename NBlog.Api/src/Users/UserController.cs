using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using NBlog.Api.Users;
using NBlog.Api.Likes;

namespace NBlog.Api.Controllers;

[Authorize]
[EnableRateLimiting("token")]
[ApiController]
[Route("/Api/V1/[controller]s")]
public sealed class UserController(
    ILikeRepository likeRepository,
    IUserRepository userRepository,
    ILogger<UserController> logger) : ControllerBase
{

    [HttpGet("{username}")]
    public async Task<IActionResult> GetCurrentUserDetails(string username)
    {
        logger.LogInformation("fetching user {} details", username);
        var user = await userRepository.GetByName(username);

        var res = new UserDetailsDto(
            user.Id,
            user.UserName!,
            user.Email!,
            user.EmailConfirmed,
            user.PhoneNumber!,
            user.PhoneNumberConfirmed);

        logger.LogInformation("user {} details were fetched successfully", username);
        return Ok(res);
    }

    [HttpGet("{username}/[action]")]
    public async Task<IActionResult> Likes(string username)
    {
        logger.LogInformation("fetching likes count for user {}", username);
        var res = await likeRepository.GetLikesCountForUser(username);
        logger.LogInformation("user {} has {} likes", username, res.Count);
        return Ok(res);
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> Follow([FromBody] FollowReq req)
    {
        var action = await userRepository.Follow(req.UserName, req.OtherUserName);
        return action switch
        {
            FollowAction.Follow => Ok($"user {req.UserName} has followed {req.OtherUserName} successfully"),
            FollowAction.Unfollow => Ok($"user {req.UserName} has unfollowed {req.OtherUserName} successfully"),
            _ => BadRequest("Unknown action")
        };
    }
}