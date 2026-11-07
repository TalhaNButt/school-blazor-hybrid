using MyHybridApp2.Domain.Common;

namespace MyHybridApp2.Domain.Posts;

public sealed class Post : Entity
{
    public const int AuthorNameMaxLength = 100;
    public const int ContentTextMaxLength = 5000;

    // Required by EF Core.
    private Post() { }

    public string AuthorName { get; private set; } = string.Empty;
    public string? ContentText { get; private set; }
    public string? MediaUrl { get; private set; }
    public string? MediaType { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public int LikeCount { get; private set; }
    public int ShareCount { get; private set; }
    public int FavoriteCount { get; private set; }

    public static Post Create(string authorName, string? contentText, string? mediaUrl, string? mediaType, DateTime createdAtUtc)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(authorName);

        return new Post
        {
            AuthorName = authorName.Trim(),
            ContentText = contentText,
            MediaUrl = mediaUrl,
            MediaType = mediaType,
            CreatedAt = createdAtUtc
        };
    }

    public void Like() => LikeCount++;

    public void Unlike() => LikeCount = Math.Max(0, LikeCount - 1);

    public void Favorite() => FavoriteCount++;

    public void Unfavorite() => FavoriteCount = Math.Max(0, FavoriteCount - 1);

    public void Share() => ShareCount++;
}
