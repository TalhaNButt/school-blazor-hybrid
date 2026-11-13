using MyHybridApp2.Domain.Posts;
using MyHybridApp2.Shared.Posts;

namespace MyHybridApp2.Application.Posts;

internal static class PostMappings
{
    public static PostResponse ToResponse(this Post post) => new(
        post.Id,
        post.AuthorName,
        GetPostType(post.MediaType),
        post.ContentText,
        post.MediaUrl,
        post.MediaType,
        post.CreatedAt,
        post.LikeCount,
        post.ShareCount,
        post.FavoriteCount);

    private static PostType GetPostType(string? mediaType) => mediaType switch
    {
        not null when mediaType.StartsWith("image/", StringComparison.OrdinalIgnoreCase) => PostType.Image,
        not null when mediaType.StartsWith("video/", StringComparison.OrdinalIgnoreCase) => PostType.Video,
        _ => PostType.Text
    };
}
