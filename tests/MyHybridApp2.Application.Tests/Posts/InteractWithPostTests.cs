using MyHybridApp2.Application.Common;
using MyHybridApp2.Application.Posts;
using MyHybridApp2.Domain.Posts;

namespace MyHybridApp2.Application.Tests.Posts;

public sealed class InteractWithPostTests
{
    [Fact]
    public async Task Like_ThenUnlike_ReturnsToZero_AndNeverGoesNegative()
    {
        await using var db = TestDbContext.Create();
        var post = Post.Create("Jane", "Hello", null, null, DateTime.UtcNow);
        db.Posts.Add(post);
        await db.SaveChangesAsync(TestContext.Current.CancellationToken);
        var handler = new InteractWithPost(db);
        var ct = TestContext.Current.CancellationToken;

        var liked = await handler.HandleAsync(post.Id, PostInteraction.Like, ct);
        await handler.HandleAsync(post.Id, PostInteraction.Unlike, ct);
        var result = await handler.HandleAsync(post.Id, PostInteraction.Unlike, ct);

        Assert.Equal(1, liked.LikeCount);
        Assert.Equal(0, result.LikeCount);
    }

    [Fact]
    public async Task UnknownPost_ThrowsNotFound()
    {
        await using var db = TestDbContext.Create();
        var handler = new InteractWithPost(db);

        await Assert.ThrowsAsync<NotFoundException>(() =>
            handler.HandleAsync(Guid.NewGuid(), PostInteraction.Share, TestContext.Current.CancellationToken));
    }
}
