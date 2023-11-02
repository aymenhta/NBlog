namespace NBlog.Api.Entities;

public class Category
{
    public long Id { get; set; }
    public string Name { get; set; } = default!;
    public IList<Post> Posts { get; set; } = new List<Post>();
}