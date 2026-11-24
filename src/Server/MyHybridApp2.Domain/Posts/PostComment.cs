using MyHybridApp2.Domain.Common;

namespace MyHybridApp2.Domain.Posts;

public sealed class PostComment : Entity
{
    public const int AuthorNameMaxLength = 100;
    public const int ContentMaxLength = 2000;

    private PostComment() { }

    public Guid PostId { get; private set; }
    public string AuthorName { get; private set; } = string.Empty;
    public string Content { get; private set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; private set; }

    public static PostComment Create(Guid postId, string authorName, string content, DateTimeOffset createdAt)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(authorName);
        ArgumentException.ThrowIfNullOrWhiteSpace(content);

        return new PostComment
        {
            PostId = postId,
            AuthorName = authorName.Trim(),
            Content = content.Trim(),
            CreatedAt = createdAt
        };
    }
}
