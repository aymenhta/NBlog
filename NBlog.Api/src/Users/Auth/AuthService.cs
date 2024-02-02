using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace NBlog.Api.Users;

public class AuthService(
    IConfiguration configuration,
    RoleManager<IdentityRole> roleManager,
    UserManager<AppUser> userManager) : IAuthService
{

    public async Task<(int, string)> Registration(RegistrationModel model, string role)
    {
        var userExists = await userManager.FindByNameAsync(model.Username);

        if (userExists is not null)
            return (0, "user already exists");

        var user = new AppUser
        {
            Id = Guid.NewGuid().ToString(),
            Email = model.Email,
            UserName = model.Username,
            SecurityStamp = Guid.NewGuid().ToString()
        };

        var createUserResult = await userManager.CreateAsync(user, model.Password);
        if (!createUserResult.Succeeded)
            return (0, "user creation has failed, please check login details");

        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole(role));

        if (await roleManager.RoleExistsAsync(role))
            await userManager.AddToRoleAsync(user, role);

        return (1, "user was created successfully");
    }

    public async Task<(int, string)> Login(LoginModel model)
    {
        var user = await userManager.FindByNameAsync(model.Username);
        if (user is null || !await userManager.CheckPasswordAsync(user, model.Password))
            return (0, "wrong credentials");

        var userRoles = await userManager.GetRolesAsync(user);
        var authClaims = new List<Claim>
        {
            new(ClaimTypes.Name, user.UserName!),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new("uid", user.Id)
        };

        foreach (var role in userRoles)
            authClaims.Add(new Claim(ClaimTypes.Role, role));

        var token = GenerateToken(authClaims);
        return (1, token);
    }

    private string GenerateToken(IEnumerable<Claim> claims)
    {
        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWTKey:Secret"]!));
        var tokenExpiryTimeInHour = Convert.ToInt64(configuration["JWTKey:TokenExpiryTimeInHour"]!);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Issuer = configuration["JWTKey:ValidIssuer"],
            Audience = configuration["JWTKey:ValidAudience"],
            // Expires = DateTime.UtcNow.AddHours(tokenExpiryTimeInHour),
            Expires = DateTime.UtcNow.AddMinutes(10),
            SigningCredentials = new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256),
            Subject = new ClaimsIdentity(claims)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}