namespace MyHybridApp2.Api.Entities;

public class PostEntity
{
    public Guid Id { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public string? ContentText { get; set; }
    public string? MediaUrl { get; set; }
    public string? MediaType { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public int LikeCount { get; set; }
    public int ShareCount { get; set; }
    public int FavoriteCount { get; set; }
}