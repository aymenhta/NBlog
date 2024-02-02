using Microsoft.EntityFrameworkCore;
using NBlog.Api.Entities;
using NBlog.Api.Exceptions;
using NBlog.Api.Posts;
using NBlog.Api.Users;

namespace NBlog.Api.Reviews;

public class ReviewRepository(AppDbCtx ctx) : IReviewRepository
{

    public async Task<GetReviewDetails> Create(CreateReviewReq req, AppUser author)
    {
        var post = await ctx.Posts.FirstOrDefaultAsync(p => p.Id.Equals(req.PostId));

        var review = new Review
        {
            Content = req.Content,
            Value = req.Value,
            PublishedAt = DateTime.Now,
            Post = post,
            Author = author
        };

        ctx.Reviews.Add(review);
        await ctx.SaveChangesAsync();
        return new GetReviewDetails(
            review.Id,
            Content: review.Content,
            Value: review.Value,
            PublishedAt: review.PublishedAt,
            EditedAt: review.EditedAt,
            PostId: review.PostId,
            AuthorId: review.AuthorId);
    }

    public async Task<GetReviewDetails> Edit(long id, CreateReviewReq req)
    {
        var review = await ctx.Reviews.FirstOrDefaultAsync(r => r.Id.Equals(id)) ??
                     throw new ResourceNotFoundException();

        review.Content = req.Content;
        review.Value = req.Value;
        review.EditedAt = DateTime.Now;

        await ctx.SaveChangesAsync();

        return new GetReviewDetails(
            review.Id,
            Content: review.Content,
            Value: review.Value,
            PublishedAt: review.PublishedAt,
            EditedAt: review.EditedAt,
            PostId: review.PostId,
            AuthorId: review.AuthorId);
    }

    public async Task<List<GetReviewDetails>> GetAll(PagingMetadata metadata)
    {
        return await ctx.Reviews
            .OrderByDescending(p => p.PublishedAt)
            .Skip(metadata.Offset)
            .Take(metadata.PageSize)
            .Select(review => new GetReviewDetails(
                review.Id,
                review.Value,
                review.Content,
                review.PublishedAt,
                review.EditedAt,
                review.PostId,
                review.AuthorId)).ToListAsync();
    }

    public async Task<GetReviewDetails> Get(long id)
    {
        var review = await ctx.Reviews.FirstOrDefaultAsync(r => r.Id.Equals(id)) ??
                     throw new ResourceNotFoundException();
        return new GetReviewDetails(
            review.Id,
            Content: review.Content,
            Value: review.Value,
            PublishedAt: review.PublishedAt,
            EditedAt: review.EditedAt,
            PostId: review.PostId,
            AuthorId: review.AuthorId);
    }

    public async Task Delete(long id)
    {
        var review = await ctx.Reviews.FirstOrDefaultAsync(r => r.Id.Equals(id)) ??
                     throw new ResourceNotFoundException();

        ctx.Reviews.Remove(review);
        await ctx.SaveChangesAsync();
    }

    public async Task<List<GetReviewDetails>> GetReviewsForPost(long postId)
    {
        return await ctx.Reviews
            .Where(r => r.PostId.Equals(postId))
            .Select(review => new GetReviewDetails(
                review.Id,
                review.Value,
                review.Content,
                review.PublishedAt,
                review.EditedAt,
                review.PostId,
                review.AuthorId))
            .ToListAsync();
    }
}