namespace NBlog.Api.Dtos;

public record GetCommentDetails(
    long Id,
    string Content,
    DateTime PublishedAt,
    DateTime EditedAt,
    long PostId,
    string AuthorId);