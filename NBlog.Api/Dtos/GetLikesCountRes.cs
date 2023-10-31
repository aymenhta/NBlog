namespace NBlog.Api.Dtos;

public record GetPostLikesCountRes(
    long PostId, int Count);

public record GetUserLikesCountRes(
    string Username, int Count);