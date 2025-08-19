using System.ComponentModel.DataAnnotations;

namespace NBlog.Api.Posts;

public record CreatePostReq(
    string Title,
    string Content,
    long CategoryId);

public record GetPostDetails(
    long Id,
    string Title,
    string Content,
    DateTime PublishedAt,
    DateTime EditedAt,
    string AuthorId,
    string Category);
