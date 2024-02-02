using System.ComponentModel.DataAnnotations;

namespace NBlog.Api.Reviews;

public record GetReviewDetails(
    long Id,
    int Value,
    string Content,
    DateTime PublishedAt,
    DateTime EditedAt,
    long PostId,
    string AuthorId);

public record CreateReviewReq(
    [Required] string Content,
    [Required][Range(1, 10)] int Value,
    [Required] long PostId);