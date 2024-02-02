using NBlog.Api.Likes;

namespace NBlog.Api.Likes;

public record GetPostLikesCountRes(
    long PostId, int Count);

public record GetUserLikesCountRes(
    string Username, int Count);

public record ReactReq(
    long PostId, LikeAction Action);