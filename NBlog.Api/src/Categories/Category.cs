using NBlog.Api.Posts;

namespace NBlog.Api.Categories;

public class Category
{
    public long Id { get; set; }
    public string Name { get; set; } = default!;
    public DateTime AddedAt { get; set; }
    public DateTime EditedAt { get; set; }
    public IList<Post> Posts { get; set; } = [];
}