using MyHybridApp2.Shared.Posts;

namespace MyHybridApp2.UI.Models;

/// <summary>
/// UI view model for a post: the API's <see cref="PostResponse"/> plus client-only state.
/// </summary>
public sealed class Post
{
    public Guid Id { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public string? AuthorAvatarUrl { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public PostType Type { get; set; }
    public string? ContentText { get; set; }
    public string? MediaUrl { get; set; }
    public string? MediaType { get; set; }
    public int LikeCount { get; set; }
    public int ShareCount { get; set; }
    public int FavoriteCount { get; set; }
    public bool IsLiked { get; set; }
    public bool IsFavorited { get; set; }
    public bool IsShared { get; set; }
}
