using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using NBlog.Api.Users;
using NBlog.Api.Posts;

namespace NBlog.Api.Reviews;

[ApiController]
[Route("/Api/V1/[controller]s")]
[Authorize]
[EnableRateLimiting("token")]
public class ReviewController(
    IReviewRepository reviewRepository,
    IUserRepository userRepository,
    ILogger<ReviewController> logger) : ControllerBase
{

    [HttpGet]
    public async Task<IActionResult> Index([FromQuery] PagingMetadata metadata)
    {
        logger.LogInformation("fetching reviews");
        var reviews = await reviewRepository.GetAll(metadata);
        logger.LogInformation("reviews were fetched successfully");
        return Ok(reviews);
    }

    [HttpGet("{id:long}")]
    public async Task<IActionResult> Details(long id)
    {
        logger.LogInformation("fetching review {} details", id);
        var review = await reviewRepository.Get(id);
        logger.LogInformation("review {} details were fetched successfully", id);
        return Ok(review);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateReviewReq req)
    {
        var author = await userRepository.GetById(User.GetCurrentUserId());

        logger.LogInformation("adding review for user {}", author.UserName);
        var review = await reviewRepository.Create(req, author);
        logger.LogInformation("review {} was added successfully for author {}", review.Id, author.UserName);
        return Ok(review);
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Edit(long id, CreateReviewReq req)
    {
        logger.LogInformation("editing review {}", id);
        var updatedReview = await reviewRepository.Edit(id, req);
        logger.LogInformation("review {} was edited successfully", id);
        return Ok(updatedReview);
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id)
    {
        logger.LogInformation("deleting review {}", id);
        await reviewRepository.Delete(id);
        logger.LogInformation("review {} was deleted successfully", id);
        return Ok();
    }
}