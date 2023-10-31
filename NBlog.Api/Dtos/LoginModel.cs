using System.ComponentModel.DataAnnotations;

namespace NBlog.Api.Dtos;

public record LoginModel(
    [Required(ErrorMessage = "Username is required")]
    string Username,
    [Required(ErrorMessage = "Password is required")]
    string Password);