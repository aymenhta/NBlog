using NBlog.Api.Entities;

namespace NBlog.Api.Dtos;

public record ReactReq(
    long PostId, LikeAction Action);