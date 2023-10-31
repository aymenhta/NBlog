using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using NBlog.Api.Dtos;
using NBlog.Api.Entities;
using NBlog.Api.Repository;

namespace NBlog.Api.Controllers;

[Authorize]
[EnableRateLimiting("token")]
[ApiController]
[Route("/Api/V1/[controller]s")]
public class UserController : ControllerBase
{
    private readonly ILikeRepository _likeRepository;
    private readonly ILogger<UserController> _logger;
    private readonly IUserRepository _userRepository;

    public UserController(ILogger<UserController> logger,
        ILikeRepository likeRepository,
        IUserRepository userRepository)
    {
        _logger = logger;
        _likeRepository = likeRepository;
        _userRepository = userRepository;
    }

    [HttpGet("{username}")]
    public async Task<IActionResult> GetCurrentUserDetails(string username)
    {
        _logger.LogInformation("fetching user {} details", username);
        var user = await _userRepository.GetByName(username);

        var res = new UserDetailsDto(
            user.Id,
            user.UserName!,
            user.Email!,
            user.EmailConfirmed,
            user.PhoneNumber!,
            user.PhoneNumberConfirmed);

        _logger.LogInformation("user {} details were fetched successfully", username);
        return Ok(res);
    }

    [HttpGet("{username}/[action]")]
    public async Task<IActionResult> Likes(string username)
    {
        _logger.LogInformation("fetching likes count for user {}", username);
        var res = await _likeRepository.GetLikesCountForUser(username);
        _logger.LogInformation("user {} has {} likes", username, res.Count);
        return Ok(res);
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> Follow([FromBody] FollowReq req)
    {
        var action = await _userRepository.Follow(req.UserName, req.OtherUserName);
        return action switch
        {
            FollowAction.Follow => Ok($"user {req.UserName} has followed {req.OtherUserName} successfully"),
            FollowAction.Unfollow => Ok($"user {req.UserName} has unfollowed {req.OtherUserName} successfully"),
            _ => BadRequest("Unknown action")
        };
    }
}