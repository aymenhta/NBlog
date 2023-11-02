using System.ComponentModel.DataAnnotations;

namespace NBlog.Api.Dtos;

public record CreateCategoryReq(
    [Required, MinLength(3), MaxLength(34)]
    string Name);