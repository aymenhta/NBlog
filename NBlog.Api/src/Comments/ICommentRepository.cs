using NBlog.Api.Categories;
using NBlog.Api.Posts;
using NBlog.Api.Users;

namespace NBlog.Api.Comments;

public interface ICommentRepository
{
    Task<List<GetCommentDetails>> GetAll(PagingMetadata metadata);
    Task<GetCommentDetails> Get(long id);
    Task<GetCommentDetails> Create(CreateCommentReq req, AppUser author);
    Task<List<GetCommentDetails>> GetCommentsForPost(long postId);
    Task Delete(long id);
    Task<GetCommentDetails> Edit(long id, CreateCommentReq req);
}