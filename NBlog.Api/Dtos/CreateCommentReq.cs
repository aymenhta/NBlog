using System.ComponentModel.DataAnnotations;

namespace NBlog.Api.Dtos;

public record CreateCommentReq(
    [Required] [MaxLength(200)] string Content,
    long PostId);