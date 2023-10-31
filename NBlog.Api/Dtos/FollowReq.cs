using NBlog.Api.Entities;

namespace NBlog.Api.Dtos;

public record FollowReq(
    string UserId, string OtherUserId, FollowAction Action);