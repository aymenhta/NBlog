namespace NBlog.Api.Dtos;

public record GetPostDetails(
    long Id,
    string Title,
    string Content,
    DateTime PublishedAt,
    DateTime EditedAt,
    string AuthorId,
    string Category);