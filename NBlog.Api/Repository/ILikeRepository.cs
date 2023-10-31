using NBlog.Api.Dtos;
using NBlog.Api.Entities;

namespace NBlog.Api.Repository;

public interface ILikeRepository
{
    Task<bool> Like(LikeAction action, long postId, string userId);
    Task<GetPostLikesCountRes> GetLikesCountForPost(long postId);
    Task<GetUserLikesCountRes> GetLikesCountForUser(string username);
}