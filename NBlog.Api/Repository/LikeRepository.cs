using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NBlog.Api.Dtos;
using NBlog.Api.Entities;
using NBlog.Api.Exceptions;

namespace NBlog.Api.Repository;

public class LikeRepository : ILikeRepository
{
    private readonly AppDbCtx _ctx;
    private readonly UserManager<AppUser> _userManager;

    public LikeRepository(AppDbCtx ctx, UserManager<AppUser> userManager)
    {
        _ctx = ctx;
        _userManager = userManager;
    }

    public async Task<bool> Like(LikeAction action, long postId, string userId)
    {
        var post = await _ctx.Posts.FirstOrDefaultAsync(p => p.Id.Equals(postId))
                   ?? throw new ResourceNotFoundException($"post #{postId} could not e found :/");
        var user = await _userManager.FindByIdAsync(userId)
                   ?? throw new ResourceNotFoundException($"user #{userId} could not e found :/");
        Like? like;
        switch (action)
        {
            case LikeAction.Like:
                // check if the user has already liked the post
                like = await GetLikeAsync(postId, userId);
                if (like is not null) return true;

                like = new Like
                {
                    User = user,
                    Post = post
                };
                _ctx.Likes.Add(like);
                await _ctx.SaveChangesAsync();
                return true;
            case LikeAction.Dislike:
                like = await GetLikeAsync(postId, userId)
                       ?? throw new ResourceNotFoundException(
                           $"like with user #{userId} and post #{postId} could not e found :/");
                _ctx.Likes.Remove(like);
                await _ctx.SaveChangesAsync();
                return false;
            default:
                throw new ArgumentOutOfRangeException(nameof(action), action, null);
        }
    }

    public async Task<GetPostLikesCountRes> GetLikesCountForPost(long postId)
    {
        var post = await _ctx.Posts.FirstOrDefaultAsync(p => p.Id.Equals(postId))
                   ?? throw new ResourceNotFoundException($"post #{postId} could not e found :/");

        var count = await _ctx.Likes.CountAsync(l => l.PostId.Equals(post.Id));
        return new GetPostLikesCountRes(PostId: post.Id, Count: count);
    }

    public async Task<GetUserLikesCountRes> GetLikesCountForUser(string username)
    {
        var user = await _userManager.FindByNameAsync(username)
                   ?? throw new ResourceNotFoundException($"user #{username} could not be found :/");
        var count = await _ctx.Likes.CountAsync(l => l.UserId.Equals(user.Id));
        return new GetUserLikesCountRes(Username: user.UserName!, Count: count);
    }

    private async Task<Like?> GetLikeAsync(long postId, string userId)
    {
        return await _ctx.Likes
            .FirstOrDefaultAsync(l => l.UserId.Equals(userId) && l.PostId.Equals(postId));
    }
}