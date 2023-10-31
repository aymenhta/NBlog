using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace NBlog.Api.Entities;

public class Review
{
    public long Id { get; set; }

    [MaxLength(10)] [MinLength(1)] public int Value { get; set; }

    public string Content { get; set; } = string.Empty;
    public DateTime PublishedAt { get; set; }
    public DateTime EditedAt { get; set; }

    [JsonIgnore] public Post? Post { get; set; }

    public long PostId { get; set; }

    [JsonIgnore] public AppUser? Author { get; set; }

    public string AuthorId { get; set; } = default!;
}