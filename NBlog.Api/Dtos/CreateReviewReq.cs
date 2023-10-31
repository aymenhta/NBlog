using System.ComponentModel.DataAnnotations;

namespace NBlog.Api.Dtos;

public record CreateReviewReq(
    [Required] string Content,
    [Required] [Range(1, 10)] int Value,
    [Required] long PostId);