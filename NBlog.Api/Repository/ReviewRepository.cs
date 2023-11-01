using Microsoft.EntityFrameworkCore;
using NBlog.Api.Dtos;
using NBlog.Api.Entities;
using NBlog.Api.Exceptions;

namespace NBlog.Api.Repository;

public class ReviewRepository : IReviewRepository
{
    private readonly AppDbCtx _ctx;

    public ReviewRepository(AppDbCtx ctx)
    {
        _ctx = ctx;
    }

    public async Task<GetReviewDetails> Create(CreateReviewReq req, AppUser author)
    {
        var post = await _ctx.Posts.FirstOrDefaultAsync(p => p.Id.Equals(req.PostId));

        var review = new Review
        {
            Content = req.Content,
            Value = req.Value,
            PublishedAt = DateTime.Now,
            Post = post,
            Author = author
        };

        _ctx.Reviews.Add(review);
        await _ctx.SaveChangesAsync();
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
        var review = await _ctx.Reviews.FirstOrDefaultAsync(r => r.Id.Equals(id)) ??
                     throw new ResourceNotFoundException();

        review.Content = req.Content;
        review.Value = req.Value;
        review.EditedAt = DateTime.Now;

        await _ctx.SaveChangesAsync();

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
        return await _ctx.Reviews
            .OrderByDescending(p => p.PublishedAt)
            .Skip((metadata.PageNumber - 1) * metadata.PageSize)
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
        var review = await _ctx.Reviews.FirstOrDefaultAsync(r => r.Id.Equals(id)) ??
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
        var review = await _ctx.Reviews.FirstOrDefaultAsync(r => r.Id.Equals(id)) ??
                     throw new ResourceNotFoundException();

        _ctx.Reviews.Remove(review);
        await _ctx.SaveChangesAsync();
    }

    public async Task<List<GetReviewDetails>> GetReviewsForPost(long postId)
    {
        return await _ctx.Reviews
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