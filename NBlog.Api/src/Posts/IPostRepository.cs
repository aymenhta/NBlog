using NBlog.Api.Users;

namespace NBlog.Api.Posts;

public interface IPostRepository
{
    Task<List<GetPostDetails>> GetAll(PagingMetadata metadata);
    Task<GetPostDetails> Get(long id);
    Task<GetPostDetails> Save(CreatePostReq req, AppUser identityUser);
    Task<GetPostDetails> Edit(long id, CreatePostReq req);
    Task Delete(long id);
}