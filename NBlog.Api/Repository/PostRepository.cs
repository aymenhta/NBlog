using Microsoft.EntityFrameworkCore;
using NBlog.Api.Dtos;
using NBlog.Api.Entities;
using NBlog.Api.Exceptions;

namespace NBlog.Api.Repository;

public class PostRepository : IPostRepository
{
    private readonly AppDbCtx _ctx;

    public PostRepository(AppDbCtx ctx)
    {
        _ctx = ctx;
    }

    public async Task<List<GetPostDetails>> GetAll()
    {
        return await _ctx.Posts
            .Select(post => new GetPostDetails(
                post.Id,
                post.Title,
                post.Content,
                post.PublishedAt,
                post.EditedAt,
                post.AuthorId))
            .ToListAsync();
    }

    public async Task<GetPostDetails> Get(long id)
    {
        var post = await _ctx.Posts.FirstOrDefaultAsync(p => p.Id == id) ?? throw new ResourceNotFoundException();
        return new GetPostDetails(
            post.Id,
            post.Title,
            post.Content,
            post.PublishedAt,
            post.EditedAt,
            post.AuthorId);
    }

    public async Task<GetPostDetails> Save(CreatePostReq req, AppUser author)
    {
        var post = new Post
        {
            Title = req.Title, Content = req.Content, PublishedAt = DateTime.Now, EditedAt = DateTime.Now,
            AuthorId = author.Id
        };

        _ctx.Posts.Add(post);
        await _ctx.SaveChangesAsync();
        return new GetPostDetails(
            post.Id,
            post.Title,
            post.Content,
            post.PublishedAt,
            post.EditedAt,
            post.AuthorId);
    }

    public async Task<GetPostDetails> Edit(long id, CreatePostReq req)
    {
        var post = await _ctx.Posts.FirstOrDefaultAsync(p => p.Id.Equals(id)) ?? throw new ResourceNotFoundException();
        post.Title = req.Title;
        post.Content = req.Content;

        await _ctx.SaveChangesAsync();

        return new GetPostDetails(
            post.Id,
            post.Title,
            post.Content,
            post.PublishedAt,
            post.EditedAt,
            post.AuthorId);
    }

    public async Task Delete(long id)
    {
        var post = await _ctx.Posts.FirstOrDefaultAsync(p => p.Id.Equals(id)) ?? throw new ResourceNotFoundException();
        _ctx.Posts.Remove(post);
        await _ctx.SaveChangesAsync();
    }
}