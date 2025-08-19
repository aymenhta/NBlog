using System.ComponentModel.DataAnnotations;

namespace NBlog.Api.Categories;

public record GetCommentDetails(
    long Id,
    string Content,
    DateTime PublishedAt,
    DateTime EditedAt,
    long PostId,
    string AuthorId);

public record CreateCommentReq(string Content, long PostId);
