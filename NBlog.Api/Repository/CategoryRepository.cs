using Microsoft.EntityFrameworkCore;
using NBlog.Api.Dtos;
using NBlog.Api.Entities;
using NBlog.Api.Exceptions;

namespace NBlog.Api.Repository;

public class CategoryRepository : ICategoryRepository
{
    private readonly AppDbCtx _ctx;

    public CategoryRepository(AppDbCtx ctx)
    {
        _ctx = ctx;
    }

    public async Task<List<GetPostDetails>> GetPosts(PagingMetadata metadata, string name)
    {
        return await _ctx.Posts
            .Include(p => p.Category)
            .Where(p => p.Category!.Name.Equals(name.ToUpper()))
            .OrderByDescending(p => p.PublishedAt)
            .Skip((metadata.PageNumber - 1) * metadata.PageSize)
            .Take(metadata.PageSize)
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

    public async Task<GetCategoryDetails> Create(CreateCategoryReq req)
    {
        var category = new Category { Name = req.Name.ToUpper(), AddedAt = DateTime.Now, EditedAt = DateTime.Now };
        _ctx.Categories.Add(category);
        await _ctx.SaveChangesAsync();
        return new GetCategoryDetails(Id: category.Id, Name: category.Name, PostsCount: 0);
    }

    public async Task<GetCategoryDetails> Edit(CreateCategoryReq req, long id)
    {
        var category = await _ctx.Categories.FirstOrDefaultAsync(c => c.Id.Equals(id)) ??
                       throw new ResourceNotFoundException($"category {id} was not found :/");

        category.Name = req.Name.ToUpper();
        category.EditedAt = DateTime.Now;
        await _ctx.SaveChangesAsync();

        return new GetCategoryDetails(Id: category.Id, Name: category.Name, PostsCount: 0);
    }

    public async Task<GetCategoryDetails> GetByName(string name)
    {
        var category = await _ctx.Categories.FirstOrDefaultAsync(c => c.Name.Equals(name.ToUpper())) ??
                       throw new ResourceNotFoundException($"category {name} was not found :/");
        var postsCount = await GetPostsCount(name.ToUpper());
        return new GetCategoryDetails(Id: category.Id, Name: category.Name, PostsCount: postsCount);
    }

    public async Task Delete(long id)
    {
        var category = await _ctx.Categories.FirstOrDefaultAsync(c => c.Id.Equals(id)) ??
                       throw new ResourceNotFoundException($"category {id} was not found :/");
        _ctx.Categories.Remove(category);
        await _ctx.SaveChangesAsync();
    }

    private async Task<int> GetPostsCount(string name)
    {
        return await _ctx.Posts
            .Include(p => p.Category)
            .CountAsync(p => p.Category!.Name.Equals(name));
    }
}