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
public class PostController : ControllerBase
{
    private readonly ICommentRepository _commentRepository;
    private readonly ILikeRepository _likeRepository;
    private readonly ILogger<PostController> _logger;
    private readonly IPostRepository _postRepository;
    private readonly IReviewRepository _reviewRepository;
    private readonly IUserRepository _userRepository;

    public PostController(IPostRepository postRepository,
        ICommentRepository commentRepository,
        IReviewRepository reviewRepository,
        ILikeRepository likeRepository,
        ILogger<PostController> logger,
        IUserRepository userRepository)
    {
        _postRepository = postRepository;
        _commentRepository = commentRepository;
        _reviewRepository = reviewRepository;
        _likeRepository = likeRepository;
        _logger = logger;
        _userRepository = userRepository;
    }

    [HttpGet]
    public async Task<IActionResult> Index([FromQuery] PagingMetadata metadata)
    {
        var result = await _postRepository.GetAll(metadata);
        _logger.LogInformation("posts has been fetched successfully");
        return Ok(result);
    }

    [HttpGet("{id:long}")]
    public async Task<IActionResult> Details(long id)
    {
        var post = await _postRepository.Get(id);
        _logger.LogInformation("post {} has been fetched successfully", post.Id);
        return Ok(post);
    }

    [HttpGet("{id:long}/Comments")]
    public async Task<IActionResult> Comments(long id)
    {
        var comments = await _commentRepository.GetCommentsForPost(id);
        _logger.LogInformation("comments for post {} has been fetched successfully", id);
        return Ok(comments);
    }

    [HttpGet("{id:long}/Reviews")]
    public async Task<IActionResult> Reviews(long id)
    {
        var reviews = await _reviewRepository.GetReviewsForPost(id);
        _logger.LogInformation("reviews for post {} has been fetched successfully", id);
        return Ok(reviews);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreatePostReq req)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("failed validation");
            return BadRequest(ModelState);
        }

        var author = await _userRepository.GetById(User.GetCurrentUserId());
        _logger.LogInformation("publishing post for user: {}", author.UserName);
        var post = await _postRepository.Save(req, author);
        _logger.LogInformation("post {} for user: {} has been published", post.Id, author.UserName);
        return Ok(post);
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Edit(long id, CreatePostReq req)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("failed validation");
            return BadRequest(ModelState);
        }

        _logger.LogInformation("editing post {}", id);
        var updatedPost = await _postRepository.Edit(id, req);
        _logger.LogInformation("post {} for user: {} has been published", updatedPost.Id, updatedPost.AuthorId);
        return Ok(updatedPost);
    }

    [HttpDelete("{id:long}")]
    public async Task<string> Delete(long id)
    {
        _logger.LogInformation("deleting post {}", id);
        await _postRepository.Delete(id);
        _logger.LogInformation("post {} was deleted successfully", id);
        return $"Post #{id} was deleted successfully";
    }

    [HttpPost("React")]
    public async Task<IActionResult> Like([FromBody] ReactReq req)
    {
        var user = await _userRepository.GetById(User.GetCurrentUserId());
        _logger.LogInformation("reacting to post {} by user {}", req.PostId, user.UserName);
        var result = await _likeRepository.Like(req.Action, req.PostId, user.Id);
        _logger.LogInformation("reacting to post {} by user {} was successful", req.PostId, user.UserName);
        return Ok(
            result ? $"post #{req.PostId} was liked successfully" : $"post #{req.PostId} was disliked successfully");
    }

    [HttpGet("{id:long}/likes-count")]
    public async Task<IActionResult> LikesCount(long id)
    {
        _logger.LogInformation("getting reactions count for post {}", id);
        var res = await _likeRepository.GetLikesCountForPost(id);
        _logger.LogInformation("post {} has {} reactions", id, res.Count);
        return Ok(res);
    }
}