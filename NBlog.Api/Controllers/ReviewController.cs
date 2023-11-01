using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using NBlog.Api.Dtos;
using NBlog.Api.Extensions;
using NBlog.Api.Repository;

namespace NBlog.Api.Controllers;

[ApiController]
[Route("/Api/V1/[controller]s")]
[Authorize]
[EnableRateLimiting("token")]
public class ReviewController : ControllerBase
{
    private readonly ILogger<ReviewController> _logger;
    private readonly IReviewRepository _reviewRepository;
    private readonly IUserRepository _userRepository;

    public ReviewController(IReviewRepository reviewRepository,
        ILogger<ReviewController> logger,
        IUserRepository userRepository)
    {
        _reviewRepository = reviewRepository;
        _logger = logger;
        _userRepository = userRepository;
    }

    [HttpGet]
    public async Task<IActionResult> Index([FromQuery] PagingMetadata metadata)
    {
        _logger.LogInformation("fetching reviews");
        var reviews = await _reviewRepository.GetAll(metadata);
        _logger.LogInformation("reviews were fetched successfully");
        return Ok(reviews);
    }

    [HttpGet("{id:long}")]
    public async Task<IActionResult> Details(long id)
    {
        _logger.LogInformation("fetching review {} details", id);
        var review = await _reviewRepository.Get(id);
        _logger.LogInformation("review {} details were fetched successfully", id);
        return Ok(review);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateReviewReq req)
    {
        var author = await _userRepository.GetById(User.GetCurrentUserId());

        _logger.LogInformation("adding review for user {}", author!.UserName);
        var review = await _reviewRepository.Create(req, author);
        _logger.LogInformation("review {} was added successfully for author {}", review.Id, author.UserName);
        return Ok(review);
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Edit(long id, CreateReviewReq req)
    {
        _logger.LogInformation("editing review {}", id);
        var updatedReview = await _reviewRepository.Edit(id, req);
        _logger.LogInformation("review {} was edited successfully", id);
        return Ok(updatedReview);
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id)
    {
        _logger.LogInformation("deleting review {}", id);
        await _reviewRepository.Delete(id);
        _logger.LogInformation("review {} was deleted successfully", id);
        return Ok();
    }
}