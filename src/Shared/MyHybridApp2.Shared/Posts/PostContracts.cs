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
    int FavoriteCount,
    int CommentCount = 0);

public sealed record CreatePostRequest(
    string AuthorName,
    string? ContentText,
    string? MediaUrl,
    string? MediaType);

/// <summary>Payload for adding a comment to a post.</summary>
public sealed record CreatePostCommentRequest(string AuthorName, string Content);

/// <summary>A comment attached to a post.</summary>
public sealed record PostCommentResponse(
    Guid Id,
    Guid PostId,
    string AuthorName,
    string Content,
    DateTimeOffset CreatedAt);
