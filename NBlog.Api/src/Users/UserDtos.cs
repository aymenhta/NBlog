namespace NBlog.Api.Users;

public record UserDetailsDto(
    string Id,
    string Username,
    string Email,
    bool IsEmailConfirmed,
    string PhoneNumber,
    bool IsPhoneNumberConfirmed);

public record FollowReq(
    string UserName, string OtherUserName);