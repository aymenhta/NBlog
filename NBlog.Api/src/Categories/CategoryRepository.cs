using Microsoft.EntityFrameworkCore;
using NBlog.Api.Entities;
using NBlog.Api.Exceptions;
using NBlog.Api.Posts;

namespace NBlog.Api.Categories;

public class CategoryRepository(AppDbCtx ctx) : ICategoryRepository
{
    public async Task<List<GetPostDetails>> GetPosts(PagingMetadata metadata, string name)
    {
        return await ctx.Posts
            .Include(p => p.Category)
            .Where(p => p.Category!.Name.Equals(name.ToUpper()))
            .OrderByDescending(p => p.PublishedAt)
            .Skip(metadata.Offset)
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
        ctx.Categories.Add(category);
        await ctx.SaveChangesAsync();
        return new GetCategoryDetails(Id: category.Id, Name: category.Name, PostsCount: 0);
    }

    public async Task<GetCategoryDetails> Edit(CreateCategoryReq req, long id)
    {
        var category = await ctx.Categories.FirstOrDefaultAsync(c => c.Id.Equals(id)) ??
                       throw new ResourceNotFoundException($"category {id} was not found :/");

        category.Name = req.Name.ToUpper();
        category.EditedAt = DateTime.Now;
        await ctx.SaveChangesAsync();

        return new GetCategoryDetails(Id: category.Id, Name: category.Name, PostsCount: 0);
    }

    public async Task<GetCategoryDetails> GetByName(string name)
    {
        var category = await ctx.Categories.FirstOrDefaultAsync(c => c.Name.Equals(name.ToUpper())) ??
                       throw new ResourceNotFoundException($"category {name} was not found :/");
        var postsCount = await GetPostsCount(name.ToUpper());
        return new GetCategoryDetails(Id: category.Id, Name: category.Name, PostsCount: postsCount);
    }

    public async Task Delete(long id)
    {
        var category = await ctx.Categories.FirstOrDefaultAsync(c => c.Id.Equals(id)) ??
                       throw new ResourceNotFoundException($"category {id} was not found :/");
        ctx.Categories.Remove(category);
        await ctx.SaveChangesAsync();
    }

    private async Task<int> GetPostsCount(string name)
    {
        return await ctx.Posts
            .Include(p => p.Category)
            .CountAsync(p => p.Category!.Name.Equals(name));
    }
}