namespace NBlog.Api.Dtos;

public record UserDetailsDto(string Id, string Username, string Email, bool IsEmailConfirmed, string PhoneNumber,
    bool IsPhoneNumberConfirmed);