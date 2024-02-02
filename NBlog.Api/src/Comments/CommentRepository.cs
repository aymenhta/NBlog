using Microsoft.EntityFrameworkCore;
using NBlog.Api.Categories;
using NBlog.Api.Entities;
using NBlog.Api.Exceptions;
using NBlog.Api.Posts;
using NBlog.Api.Users;

namespace NBlog.Api.Comments;

public sealed class CommentRepository(AppDbCtx ctx) : ICommentRepository
{
    public async Task<List<GetCommentDetails>> GetAll(PagingMetadata metadata)
    {
        return await ctx.Comments
            .OrderByDescending(p => p.PublishedAt)
            .Skip(metadata.Offset)
            .Take(metadata.PageSize)
            .Select(comment => new GetCommentDetails(
                comment.Id,
                comment.Content,
                comment.PublishedAt,
                comment.EditedAt,
                comment.PostId,
                comment.AuthorId))
            .ToListAsync();
    }

    public async Task<GetCommentDetails> Get(long id)
    {
        var comment = await ctx.Comments.FirstOrDefaultAsync(c => c.Id == id) ?? throw new ResourceNotFoundException();
        return new GetCommentDetails(
            comment.Id,
            comment.Content,
            comment.PublishedAt,
            comment.EditedAt,
            comment.PostId,
            comment.AuthorId);
    }

    public async Task<GetCommentDetails> Create(CreateCommentReq req, AppUser author)
    {
        var post = await ctx.Posts.FirstOrDefaultAsync(p => p.Id.Equals(req.PostId)) ??
                   throw new ResourceNotFoundException();
        var comment = new Comment
        {
            Content = req.Content,
            Post = post,
            PublishedAt = DateTime.Now,
            EditedAt = DateTime.Now,
            Author = author
        };

        ctx.Comments.Add(comment);
        await ctx.SaveChangesAsync();
        return new GetCommentDetails(
            comment.Id,
            comment.Content,
            comment.PublishedAt,
            comment.EditedAt,
            comment.PostId,
            comment.AuthorId);
    }

    public async Task<List<GetCommentDetails>> GetCommentsForPost(long postId)
    {
        return await ctx.Comments
            .Where(c => c.PostId.Equals(postId))
            .Select(comment => new GetCommentDetails(
                comment.Id,
                comment.Content,
                comment.PublishedAt,
                comment.EditedAt,
                comment.PostId,
                comment.AuthorId))
            .ToListAsync();
    }

    public async Task Delete(long id)
    {
        var comment = await ctx.Comments.FirstOrDefaultAsync(c => c.Id.Equals(id));
        if (comment is null) return;

        ctx.Comments.Remove(comment);
        await ctx.SaveChangesAsync();
    }

    public async Task<GetCommentDetails> Edit(long id, CreateCommentReq req)
    {
        var comment = await ctx.Comments.FirstOrDefaultAsync(c => c.Id == id) ?? throw new ResourceNotFoundException();

        comment.Content = req.Content;
        comment.EditedAt = DateTime.Now;

        await ctx.SaveChangesAsync();
        return new GetCommentDetails(
            comment.Id,
            comment.Content,
            comment.PublishedAt,
            comment.EditedAt,
            comment.PostId,
            comment.AuthorId);
    }
}