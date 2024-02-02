using NBlog.Api.Posts;
using NBlog.Api.Users;

namespace NBlog.Api.Reviews;

public interface IReviewRepository
{
    Task<GetReviewDetails> Create(CreateReviewReq req, AppUser author);
    Task<GetReviewDetails> Edit(long id, CreateReviewReq req);
    Task<List<GetReviewDetails>> GetAll(PagingMetadata metadata);
    Task<GetReviewDetails> Get(long id);
    Task Delete(long id);
    Task<List<GetReviewDetails>> GetReviewsForPost(long postId);
}