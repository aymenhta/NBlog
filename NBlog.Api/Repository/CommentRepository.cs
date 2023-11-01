using Microsoft.EntityFrameworkCore;
using NBlog.Api.Dtos;
using NBlog.Api.Entities;
using NBlog.Api.Exceptions;

namespace NBlog.Api.Repository;

public class CommentRepository : ICommentRepository
{
    private readonly AppDbCtx _ctx;
    private readonly IPostRepository _postRepository;

    public CommentRepository(AppDbCtx ctx, IPostRepository postRepository)
    {
        _ctx = ctx;
        _postRepository = postRepository;
    }

    public async Task<List<GetCommentDetails>> GetAll(PagingMetadata metadata)
    {
        return await _ctx.Comments
            .OrderByDescending(p => p.PublishedAt)
            .Skip((metadata.PageNumber - 1) * metadata.PageSize)
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
        var comment = await _ctx.Comments.FirstOrDefaultAsync(c => c.Id == id) ?? throw new ResourceNotFoundException();
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
        var post = await _ctx.Posts.FirstOrDefaultAsync(p => p.Id.Equals(req.PostId)) ??
                   throw new ResourceNotFoundException();
        var comment = new Comment
        {
            Content = req.Content,
            Post = post,
            PublishedAt = DateTime.Now,
            EditedAt = DateTime.Now,
            Author = author
        };

        _ctx.Comments.Add(comment);
        await _ctx.SaveChangesAsync();
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
        return await _ctx.Comments
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
        var comment = await _ctx.Comments.FirstOrDefaultAsync(c => c.Id.Equals(id));
        if (comment is null) return;

        _ctx.Comments.Remove(comment);
        await _ctx.SaveChangesAsync();
    }

    public async Task<GetCommentDetails> Edit(long id, CreateCommentReq req)
    {
        var comment = await _ctx.Comments.FirstOrDefaultAsync(c => c.Id == id) ?? throw new ResourceNotFoundException();

        comment.Content = req.Content;
        comment.EditedAt = DateTime.Now;

        await _ctx.SaveChangesAsync();
        return new GetCommentDetails(
            comment.Id,
            comment.Content,
            comment.PublishedAt,
            comment.EditedAt,
            comment.PostId,
            comment.AuthorId);
    }
}