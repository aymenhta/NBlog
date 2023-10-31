namespace NBlog.Api.Dtos;

public record AuthResult(
    string Username,
    string Token);