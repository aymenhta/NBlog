using System.ComponentModel.DataAnnotations;

namespace NBlog.Api.Categories;

public record GetCategoryDetails(
    long Id,
    string Name,
    int PostsCount);

public record CreateCategoryReq(
    [Required, MinLength(3), MaxLength(34)]
    string Name);