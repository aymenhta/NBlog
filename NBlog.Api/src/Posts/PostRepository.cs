using Microsoft.EntityFrameworkCore;
using NBlog.Api.Entities;
using NBlog.Api.Exceptions;
using NBlog.Api.Users;

namespace NBlog.Api.Posts;

public sealed class PostRepository(AppDbCtx ctx) : IPostRepository
{

    public async Task<List<GetPostDetails>> GetAll(PagingMetadata metadata)
    {
        return await ctx.Posts
            .OrderByDescending(p => p.PublishedAt)
            .Skip(metadata.Offset)
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
        var post = await ctx.Posts.Include(p => p.Category).FirstOrDefaultAsync(p => p.Id == id) ??
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
        var category = await ctx.Categories.FirstOrDefaultAsync(c => c.Id.Equals(req.CategoryId))
                       ?? throw new ResourceNotFoundException($"category '{req.CategoryId}' could not be found :/");

        var post = new Post
        {
            Title = req.Title, Content = req.Content, PublishedAt = DateTime.Now, EditedAt = DateTime.Now,
            AuthorId = author.Id, CategoryId = category.Id
        };

        ctx.Posts.Add(post);
        await ctx.SaveChangesAsync();
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
        var post = await ctx.Posts.Include(p => p.Category).FirstOrDefaultAsync(p => p.Id.Equals(id)) ??
                   throw new ResourceNotFoundException();
        post.Title = req.Title;
        post.Content = req.Content;

        await ctx.SaveChangesAsync();

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
        var post = await ctx.Posts.FirstOrDefaultAsync(p => p.Id.Equals(id)) ?? throw new ResourceNotFoundException();
        ctx.Posts.Remove(post);
        await ctx.SaveChangesAsync();
    }
}