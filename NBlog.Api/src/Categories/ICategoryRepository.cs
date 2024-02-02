using NBlog.Api.Posts;

namespace NBlog.Api.Categories;

public interface ICategoryRepository
{
    Task<List<GetPostDetails>> GetPosts(PagingMetadata metadata, string name);
    Task<GetCategoryDetails> Create(CreateCategoryReq req);
    Task<GetCategoryDetails> Edit(CreateCategoryReq req, long id);
    Task<GetCategoryDetails> GetByName(string name);
    Task Delete(long id);
}