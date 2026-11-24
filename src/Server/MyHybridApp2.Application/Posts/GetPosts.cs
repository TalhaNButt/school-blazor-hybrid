using Microsoft.EntityFrameworkCore;
using MyHybridApp2.Application.Abstractions;
using MyHybridApp2.Application.Common;
using MyHybridApp2.Domain.Posts;
using MyHybridApp2.Shared.Posts;

namespace MyHybridApp2.Application.Posts;

public sealed class GetPosts(IAppDbContext db)
{
    public async Task<IReadOnlyList<PostResponse>> HandleAsync(CancellationToken cancellationToken)
    {
        var posts = await db.Posts
            .AsNoTracking()
            .OrderByDescending(p => p.CreatedAt)
            .Select(post => new
            {
                Post = post,
                CommentCount = db.PostComments.Count(comment => comment.PostId == post.Id)
            })
            .ToListAsync(cancellationToken);

        return posts.Select(result => result.Post.ToResponse(result.CommentCount)).ToList();
    }
}

public sealed class GetPostById(IAppDbContext db)
{
    public async Task<PostResponse> HandleAsync(Guid id, CancellationToken cancellationToken)
    {
        var result = await db.Posts
            .AsNoTracking()
            .Where(post => post.Id == id)
            .Select(post => new
            {
                Post = post,
                CommentCount = db.PostComments.Count(comment => comment.PostId == post.Id)
            })
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new NotFoundException(nameof(Post), id);

        return result.Post.ToResponse(result.CommentCount);
    }
}
