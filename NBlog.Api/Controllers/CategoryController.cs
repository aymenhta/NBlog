using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using NBlog.Api.Dtos;
using NBlog.Api.Repository;

namespace NBlog.Api.Controllers;

[ApiController]
[Route("/Api/V1/Categories")]
[Authorize]
[EnableRateLimiting("token")]
public class CategoryController : ControllerBase
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly ILogger<CategoryController> _logger;

    public CategoryController(ICategoryRepository categoryRepository, ILogger<CategoryController> logger)
    {
        _categoryRepository = categoryRepository;
        _logger = logger;
    }

    [HttpGet("{name}")]
    public async Task<IActionResult> Details(string name)
    {
        _logger.LogInformation("Fetching details for category '{}'", name);
        var res = await _categoryRepository.GetByName(name);
        _logger.LogInformation("details for category '{}' were fetched successfully", name);
        return Ok(res);
    }

    [HttpGet("{name}/posts")]
    public async Task<IActionResult> Index([FromQuery] PagingMetadata metadata, string name)
    {
        _logger.LogInformation("fetching posts for category '{}'", name);
        var posts = await _categoryRepository.GetPosts(metadata, name);
        _logger.LogInformation("posts for category '{}' fetched successfully", name);
        return Ok(posts);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateCategoryReq req)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("failed validation");
            return BadRequest();
        }

        var result = await _categoryRepository.Create(req);
        return Ok(result);
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Edit(long id, CreateCategoryReq req)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("failed validation");
            return BadRequest();
        }

        var result = await _categoryRepository.Edit(req, id);
        _logger.LogInformation("category #{} was updated successfully", id);
        return Ok(result);
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id)
    {
        await _categoryRepository.Delete(id);
        _logger.LogInformation("category #{} was deleted successfully", id);
        return Ok();
    }
}