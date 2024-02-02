using System.ComponentModel.DataAnnotations;

namespace NBlog.Api.Posts;

public record CreatePostReq(
    [Required][MaxLength(200)] string Title,
    [Required][MaxLength(600)] string Content,
    [Required] long CategoryId);

public record GetPostDetails(
    long Id,
    string Title,
    string Content,
    DateTime PublishedAt,
    DateTime EditedAt,
    string AuthorId,
    string Category);