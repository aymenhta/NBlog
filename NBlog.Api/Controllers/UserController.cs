using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using NBlog.Api.Dtos;
using NBlog.Api.Repository;

namespace NBlog.Api.Controllers;

[Authorize]
[EnableRateLimiting("token")]
[ApiController]
[Route("/Api/V1/[controller]s/{username}")]
public class UserController : ControllerBase
{
    private readonly ILikeRepository _likeRepository;
    private readonly ILogger<UserController> _logger;
    private readonly UserManager<IdentityUser> _userManager;

    public UserController(UserManager<IdentityUser> userManager, ILogger<UserController> logger,
        ILikeRepository likeRepository)
    {
        _userManager = userManager;
        _logger = logger;
        _likeRepository = likeRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetCurrentUserDetails(string username)
    {
        _logger.LogInformation("fetching user {} details", username);
        var user = await _userManager.FindByNameAsync(username);
        if (user is null)
            return NotFound($"user '{username}' could not be found :/");

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

    [HttpGet("[action]")]
    public async Task<IActionResult> Likes(string username)
    {
        _logger.LogInformation("fetching likes count for user {}", username);
        var res = await _likeRepository.GetLikesCountForUser(username);
        _logger.LogInformation("user {} has {} likes", username, res.Count);
        return Ok(res);
    }
}