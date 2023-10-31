using NBlog.Api.Dtos;
using NBlog.Api.Entities;

namespace NBlog.Api.Repository;

public interface IPostRepository
{
    Task<List<GetPostDetails>> GetAll();
    Task<GetPostDetails> Get(long id);
    Task<GetPostDetails> Save(CreatePostReq req, AppUser identityUser);
    Task<GetPostDetails> Edit(long id, CreatePostReq req);
    Task Delete(long id);
}