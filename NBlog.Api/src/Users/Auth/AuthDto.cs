using System.ComponentModel.DataAnnotations;

namespace NBlog.Api.Users;

public record AuthResult(
    string Username,
    string Token);

public record LoginModel(
    [Required(ErrorMessage = "Username is required")]
    string Username,
    [Required(ErrorMessage = "Password is required")]
    string Password);

public record RegistrationModel(
    [Required] string Username,
    [Required][EmailAddress] string Email,
    [Required] string Password);