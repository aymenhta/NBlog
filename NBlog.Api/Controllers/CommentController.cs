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
public class CommentController : ControllerBase
{
    private readonly ICommentRepository _commentRepository;
    private readonly ILogger<CommentController> _logger;
    private readonly IUserRepository _userRepository;

    public CommentController(ICommentRepository commentRepository,
        ILogger<CommentController> logger,
        IUserRepository userRepository)
    {
        _commentRepository = commentRepository;
        _logger = logger;
        _userRepository = userRepository;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        _logger.LogInformation("fetching comments");
        var comments = await _commentRepository.GetAll();
        _logger.LogInformation("comments were fetched successfully");
        return Ok(comments);
    }

    [HttpGet("{id:long}")]
    public async Task<IActionResult> Details(long id)
    {
        _logger.LogInformation("fetching comment {} details", id);
        var comment = await _commentRepository.Get(id);
        _logger.LogInformation("comment {} details were fetched successfully", id);
        return Ok(comment);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateCommentReq req)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("failed validation");
            return BadRequest(ModelState);
        }

        var author = await _userRepository.GetById(User.GetCurrentUserId());

        _logger.LogInformation("adding comment for user {}", author.UserName);
        var comment = await _commentRepository.Create(req, author);
        _logger.LogInformation("comment {} was added successfully for user {}", comment.Id, author.UserName);

        return Ok(comment);
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Edit(long id, CreateCommentReq req)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("failed validation");
            return BadRequest(ModelState);
        }

        _logger.LogInformation("updating comment {}", id);
        var updatedComment = await _commentRepository.Edit(id, req);
        _logger.LogInformation("comment {} was successfully updated", updatedComment.Id);
        return Ok(updatedComment);
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id)
    {
        _logger.LogInformation("updating comment {}", id);
        await _commentRepository.Delete(id);
        _logger.LogInformation("comment {} was successfully updated", id);
        return Ok();
    }
}