namespace MyHybridApp2.Shared.Posts;

public enum PostType
{
    Text,
    Image,
    Video
}

public sealed record PostResponse(
    Guid Id,
    string AuthorName,
    PostType Type,
    string? ContentText,
    string? MediaUrl,
    string? MediaType,
    DateTime CreatedAt,
    int LikeCount,
    int ShareCount,
    int FavoriteCount);

public sealed record CreatePostRequest(
    string AuthorName,
    string? ContentText,
    string? MediaUrl,
    string? MediaType);
