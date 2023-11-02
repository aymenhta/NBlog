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

    public async Task<List<GetPostDetails>> GetAll(PagingMetadata metadata)
    {
        return await _ctx.Posts
            .OrderByDescending(p => p.PublishedAt)
            .Skip((metadata.PageNumber - 1) * metadata.PageSize)
            .Take(metadata.PageSize)
            .Include(p => p.Category)
            .Select(post => new GetPostDetails(
                post.Id,
                post.Title,
                post.Content,
                post.PublishedAt,
                post.EditedAt,
                post.AuthorId,
                post.Category!.Name))
            .ToListAsync();
    }

    public async Task<GetPostDetails> Get(long id)
    {
        var post = await _ctx.Posts.Include(p => p.Category).FirstOrDefaultAsync(p => p.Id == id) ??
                   throw new ResourceNotFoundException();
        return new GetPostDetails(
            post.Id,
            post.Title,
            post.Content,
            post.PublishedAt,
            post.EditedAt,
            post.AuthorId,
            post.Category!.Name);
    }

    public async Task<GetPostDetails> Save(CreatePostReq req, AppUser author)
    {
        var category = await _ctx.Categories.FirstOrDefaultAsync(c => c.Id.Equals(req.CategoryId))
                       ?? throw new ResourceNotFoundException($"category '{req.CategoryId}' could not be found :/");

        var post = new Post
        {
            Title = req.Title, Content = req.Content, PublishedAt = DateTime.Now, EditedAt = DateTime.Now,
            AuthorId = author.Id, CategoryId = category.Id
        };

        _ctx.Posts.Add(post);
        await _ctx.SaveChangesAsync();
        return new GetPostDetails(
            post.Id,
            post.Title,
            post.Content,
            post.PublishedAt,
            post.EditedAt,
            post.AuthorId,
            category.Name);
    }

    public async Task<GetPostDetails> Edit(long id, CreatePostReq req)
    {
        var post = await _ctx.Posts.Include(p => p.Category).FirstOrDefaultAsync(p => p.Id.Equals(id)) ??
                   throw new ResourceNotFoundException();
        post.Title = req.Title;
        post.Content = req.Content;

        await _ctx.SaveChangesAsync();

        return new GetPostDetails(
            post.Id,
            post.Title,
            post.Content,
            post.PublishedAt,
            post.EditedAt,
            post.AuthorId,
            post.Category!.Name);
    }

    public async Task Delete(long id)
    {
        var post = await _ctx.Posts.FirstOrDefaultAsync(p => p.Id.Equals(id)) ?? throw new ResourceNotFoundException();
        _ctx.Posts.Remove(post);
        await _ctx.SaveChangesAsync();
    }
}