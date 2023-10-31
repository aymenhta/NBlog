namespace NBlog.Api.Dtos;

public record FollowReq(
    string UserName, string OtherUserName);