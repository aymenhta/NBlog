namespace NBlog.Api.Dtos;

public record GetCategoryDetails(
    long Id, string Name, int PostsCount);