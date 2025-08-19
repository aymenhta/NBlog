using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace NBlog.Api.Users;


[ApiController]
[Route("/Api/V1/[controller]s")]
[EnableRateLimiting("token")]
public sealed class AuthController(
    IAuthService authService,
    ILogger<AuthController> logger) : ControllerBase
{

    [HttpPost("[action]")]
    public async Task<IActionResult> Register(
        [FromServices] IValidator<RegistrationModel> reqValidator,
        [FromBody] RegistrationModel model)
    {
        var vResult = await reqValidator.ValidateAsync(model);
        if (!vResult.IsValid)
        {
            logger.LogWarning("failed validation when adding a new user");
            return BadRequest(vResult.Errors);
        }

        var (idx, msg) = await authService.Registration(model, "USER");
        if (idx == 0)
        {
            logger.LogWarning("failed registration: {}", msg);
            return BadRequest(msg);
        }

        logger.LogInformation("user {} has been registered successfully", model.Username);
        return Ok(msg);
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> Login(
        [FromServices] IValidator<LoginModel> reqValidator,
        [FromBody] LoginModel model)
    {
        var vResult = await reqValidator.ValidateAsync(model);
        if (!vResult.IsValid)
        {
            logger.LogWarning("failed validation when adding a new user");
            return BadRequest(vResult.Errors);
        }

        var (idx, result) = await authService.Login(model);
        if (idx == 0)
        {
            logger.LogWarning(result);
            return BadRequest(result);
        }

        var authResult = new AuthResult(model.Username, result);
        logger.LogInformation("user {} has logged in successfully", authResult.Username);
        return Ok(authResult);
    }
}
