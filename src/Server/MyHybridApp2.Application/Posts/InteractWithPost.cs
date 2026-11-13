using MyHybridApp2.Application.Abstractions;
using MyHybridApp2.Application.Common;
using MyHybridApp2.Domain.Posts;
using MyHybridApp2.Shared.Posts;

namespace MyHybridApp2.Application.Posts;

public enum PostInteraction
{
    Like,
    Unlike,
    Favorite,
    Unfavorite,
    Share
}

/// <summary>
/// Updates engagement counters. Per-user state (who liked what) needs authentication and belongs here once users exist.
/// </summary>
public sealed class InteractWithPost(IAppDbContext db)
{
    public async Task<PostResponse> HandleAsync(Guid id, PostInteraction interaction, CancellationToken cancellationToken)
    {
        var post = await db.Posts.FindAsync([id], cancellationToken)
            ?? throw new NotFoundException(nameof(Post), id);

        switch (interaction)
        {
            case PostInteraction.Like: post.Like(); break;
            case PostInteraction.Unlike: post.Unlike(); break;
            case PostInteraction.Favorite: post.Favorite(); break;
            case PostInteraction.Unfavorite: post.Unfavorite(); break;
            case PostInteraction.Share: post.Share(); break;
            default: throw new ArgumentOutOfRangeException(nameof(interaction), interaction, null);
        }

        await db.SaveChangesAsync(cancellationToken);
        return post.ToResponse();
    }
}
