namespace NBlog.Api.Entities;

public class Post
{
    public long Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime PublishedAt { get; set; }
    public DateTime EditedAt { get; set; }
    public IList<Comment> Comments { get; set; } = new List<Comment>();
    public IList<Review> Reviews { get; set; } = new List<Review>();
    public AppUser? Author { get; set; }
    public string AuthorId { get; set; } = default!;
    public long CategoryId { get; set; }
    public Category? Category { get; set; }
}