using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using NBlog.Api.Dtos;
using NBlog.Api.Repository;

namespace NBlog.Api.Controllers;

[ApiController]
[Route("/Api/V1/[controller]s")]
[EnableRateLimiting("token")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> Register(RegistrationModel model)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("failed validation");
            return BadRequest(ModelState);
        }

        var (idx, msg) = await _authService.Registration(model, "USER");
        if (idx == 0)
        {
            _logger.LogWarning("failed registration: {}", msg);
            return BadRequest(msg);
        }

        _logger.LogInformation("user {} has been registered successfully", model.Username);
        return Ok(msg);
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> Login(LoginModel model)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("failed validation");
            return BadRequest(ModelState);
        }

        var (idx, result) = await _authService.Login(model);
        if (idx == 0)
        {
            _logger.LogWarning(result);
            return BadRequest(result);
        }

        var authResult = new AuthResult(model.Username, result);
        _logger.LogInformation("user {} has logged in successfully", authResult.Username);
        return Ok(authResult);
    }
}