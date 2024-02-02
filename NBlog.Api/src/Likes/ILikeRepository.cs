namespace NBlog.Api.Likes;

public interface ILikeRepository
{
    Task<bool> Like(LikeAction action, long postId, string userId);
    Task<GetPostLikesCountRes> GetLikesCountForPost(long postId);
    Task<GetUserLikesCountRes> GetLikesCountForUser(string username);
}