using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using NBlog.Api.Comments;
using NBlog.Api.Likes;
using NBlog.Api.Reviews;
using NBlog.Api.Users;

namespace NBlog.Api.Posts;

[ApiController]
[Route("/Api/V1/[controller]s")]
[Authorize]
[EnableRateLimiting("token")]
public sealed class PostController(
    ICommentRepository commentRepository,
    ILikeRepository likeRepository,
    IPostRepository postRepository,
    IReviewRepository reviewRepository,
    IUserRepository userRepository,
    ILogger<PostController> logger) : ControllerBase
{

    [HttpGet]
    public async Task<IActionResult> Index([FromQuery] PagingMetadata metadata)
    {
        var result = await postRepository.GetAll(metadata);
        logger.LogInformation("posts has been fetched successfully");
        return Ok(result);
    }

    [HttpGet("{id:long}")]
    public async Task<IActionResult> Details(long id)
    {
        var post = await postRepository.Get(id);
        logger.LogInformation("post {} has been fetched successfully", post.Id);
        return Ok(post);
    }

    [HttpGet("{id:long}/Comments")]
    public async Task<IActionResult> Comments(long id)
    {
        var comments = await commentRepository.GetCommentsForPost(id);
        logger.LogInformation("comments for post {} has been fetched successfully", id);
        return Ok(comments);
    }

    [HttpGet("{id:long}/Reviews")]
    public async Task<IActionResult> Reviews(long id)
    {
        var reviews = await reviewRepository.GetReviewsForPost(id);
        logger.LogInformation("reviews for post {} has been fetched successfully", id);
        return Ok(reviews);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreatePostReq req)
    {
        if (!ModelState.IsValid)
        {
            logger.LogWarning("failed validation");
            return BadRequest(ModelState);
        }

        var author = await userRepository.GetById(User.GetCurrentUserId());
        logger.LogInformation("publishing post for user: {}", author.UserName);
        var post = await postRepository.Save(req, author);
        logger.LogInformation("post {} for user: {} has been published", post.Id, author.UserName);
        return Ok(post);
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Edit(long id, CreatePostReq req)
    {
        if (!ModelState.IsValid)
        {
            logger.LogWarning("failed validation");
            return BadRequest(ModelState);
        }

        logger.LogInformation("editing post {}", id);
        var updatedPost = await postRepository.Edit(id, req);
        logger.LogInformation("post {} for user: {} has been published", updatedPost.Id, updatedPost.AuthorId);
        return Ok(updatedPost);
    }

    [HttpDelete("{id:long}")]
    public async Task<string> Delete(long id)
    {
        logger.LogInformation("deleting post {}", id);
        await postRepository.Delete(id);
        logger.LogInformation("post {} was deleted successfully", id);
        return $"Post #{id} was deleted successfully";
    }

    [HttpPost("React")]
    public async Task<IActionResult> Like([FromBody] ReactReq req)
    {
        var user = await userRepository.GetById(User.GetCurrentUserId());
        logger.LogInformation("reacting to post {} by user {}", req.PostId, user.UserName);
        var result = await likeRepository.Like(req.Action, req.PostId, user.Id);
        logger.LogInformation("reacting to post {} by user {} was successful", req.PostId, user.UserName);
        return Ok(
            result ? $"post #{req.PostId} was liked successfully" : $"post #{req.PostId} was disliked successfully");
    }

    [HttpGet("{id:long}/likes-count")]
    public async Task<IActionResult> LikesCount(long id)
    {
        logger.LogInformation("getting reactions count for post {}", id);
        var res = await likeRepository.GetLikesCountForPost(id);
        logger.LogInformation("post {} has {} reactions", id, res.Count);
        return Ok(res);
    }
}