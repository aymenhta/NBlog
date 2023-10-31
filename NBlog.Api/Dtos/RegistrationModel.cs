using System.ComponentModel.DataAnnotations;

namespace NBlog.Api.Dtos;

public record RegistrationModel(
    [Required] string Username,
    [Required] [EmailAddress] string Email,
    [Required] string Password);