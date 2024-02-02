using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using NBlog.Api.Comments;
using NBlog.Api.Users;
using NBlog.Api.Posts;

namespace NBlog.Api.Categories;

[ApiController]
[Route("/Api/V1/[controller]s")]
[Authorize]
[EnableRateLimiting("token")]
public sealed class CommentController(
    ICommentRepository commentRepository,
    ILogger<CommentController> logger,
    IUserRepository userRepository) : ControllerBase
{

    [HttpGet]
    public async Task<IActionResult> Index([FromQuery] PagingMetadata metadata)
    {
        logger.LogInformation("fetching comments");
        var comments = await commentRepository.GetAll(metadata);
        logger.LogInformation("comments were fetched successfully");
        return Ok(comments);
    }

    [HttpGet("{id:long}")]
    public async Task<IActionResult> Details(long id)
    {
        logger.LogInformation("fetching comment {} details", id);
        var comment = await commentRepository.Get(id);
        logger.LogInformation("comment {} details were fetched successfully", id);
        return Ok(comment);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateCommentReq req)
    {
        if (!ModelState.IsValid)
        {
            logger.LogWarning("failed validation");
            return BadRequest(ModelState);
        }

        var author = await userRepository.GetById(User.GetCurrentUserId());

        logger.LogInformation("adding comment for user {}", author.UserName);
        var comment = await commentRepository.Create(req, author);
        logger.LogInformation("comment {} was added successfully for user {}", comment.Id, author.UserName);

        return Ok(comment);
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Edit(long id, CreateCommentReq req)
    {
        if (!ModelState.IsValid)
        {
            logger.LogWarning("failed validation");
            return BadRequest(ModelState);
        }

        logger.LogInformation("updating comment {}", id);
        var updatedComment = await commentRepository.Edit(id, req);
        logger.LogInformation("comment {} was successfully updated", updatedComment.Id);
        return Ok(updatedComment);
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id)
    {
        logger.LogInformation("updating comment {}", id);
        await commentRepository.Delete(id);
        logger.LogInformation("comment {} was successfully updated", id);
        return Ok();
    }
}