using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using NBlog.Api.Posts;

namespace NBlog.Api.Categories;

[ApiController]
[Route("/Api/V1/Categories")]
[Authorize]
[EnableRateLimiting("token")]
public sealed class CategoryController(
    ICategoryRepository categoryRepository,
    IValidator<CreateCategoryReq> createCategoryValidator,
    ILogger<CategoryController> logger) : ControllerBase
{

    [HttpGet("{name}")]
    public async Task<IActionResult> Details(string name)
    {
        logger.LogInformation("Fetching details for category '{}'", name);
        var res = await categoryRepository.GetByName(name);
        logger.LogInformation("details for category '{}' were fetched successfully", name);
        return Ok(res);
    }

    [HttpGet("{name}/posts")]
    public async Task<IActionResult> Index([FromQuery] PagingMetadata metadata, string name)
    {
        logger.LogInformation("fetching posts for category '{}'", name);
        var posts = await categoryRepository.GetPosts(metadata, name);
        logger.LogInformation("posts for category '{}' fetched successfully", name);
        return Ok(posts);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateCategoryReq req)
    {
        var vResult = await createCategoryValidator.ValidateAsync(req);
        if (!vResult.IsValid)
        {
            logger.LogWarning("validation failed when creating a new category");
            return BadRequest(vResult.Errors);
        }

        var category = await categoryRepository.Create(req);
        return Ok(category);
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Edit(long id, CreateCategoryReq req)
    {
        var vResult = await createCategoryValidator.ValidateAsync(req);
        if (!vResult.IsValid)
        {
            logger.LogWarning("validation failed when editing category {}", id);
            return BadRequest(vResult.Errors);
        }

        var result = await categoryRepository.Edit(req, id);
        logger.LogInformation("category #{} was updated successfully", id);
        return Ok(result);
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id)
    {
        await categoryRepository.Delete(id);
        logger.LogInformation("category #{} was deleted successfully", id);
        return Ok();
    }
}
