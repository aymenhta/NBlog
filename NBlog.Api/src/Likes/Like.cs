using NBlog.Api.Posts;
using NBlog.Api.Users;

namespace NBlog.Api.Likes;

public sealed class Like
{
    public Guid Id { get; set; }
    public AppUser? User { get; set; }
    public string UserId { get; set; } = default!;
    public Post? Post { get; set; }
    public long PostId { get; set; }
}

public enum LikeAction
{
    Like,
    Dislike
}