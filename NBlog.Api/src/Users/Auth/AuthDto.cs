using System.ComponentModel.DataAnnotations;

namespace NBlog.Api.Users;


public record AuthResult(string Username, string Token);

public record LoginModel(string Username, string Password);

public record RegistrationModel(string Username, string Email, string Password);


