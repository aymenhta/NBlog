namespace NBlog.Api.Dtos;

public record GetReviewDetails(
    long Id,
    int Value,
    string Content,
    DateTime PublishedAt,
    DateTime EditedAt,
    long PostId,
    string AuthorId);