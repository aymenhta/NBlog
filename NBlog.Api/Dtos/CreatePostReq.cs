using System.ComponentModel.DataAnnotations;

namespace NBlog.Api.Dtos;

public record CreatePostReq(
    [Required] [MaxLength(200)] string Title,
    [Required] [MaxLength(600)] string Content,
    [Required] long CategoryId);