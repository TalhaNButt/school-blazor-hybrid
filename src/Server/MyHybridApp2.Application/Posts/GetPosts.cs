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
            .ToListAsync(cancellationToken);

        return posts.Select(p => p.ToResponse()).ToList();
    }
}

public sealed class GetPostById(IAppDbContext db)
{
    public async Task<PostResponse> HandleAsync(Guid id, CancellationToken cancellationToken)
    {
        var post = await db.Posts.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id, cancellationToken)
            ?? throw new NotFoundException(nameof(Post), id);

        return post.ToResponse();
    }
}
