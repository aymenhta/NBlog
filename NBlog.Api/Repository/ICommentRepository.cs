using NBlog.Api.Dtos;
using NBlog.Api.Entities;

namespace NBlog.Api.Repository;

public interface ICommentRepository
{
    Task<List<GetCommentDetails>> GetAll(PagingMetadata metadata);
    Task<GetCommentDetails> Get(long id);
    Task<GetCommentDetails> Create(CreateCommentReq req, AppUser author);
    Task<List<GetCommentDetails>> GetCommentsForPost(long postId);
    Task Delete(long id);
    Task<GetCommentDetails> Edit(long id, CreateCommentReq req);
}