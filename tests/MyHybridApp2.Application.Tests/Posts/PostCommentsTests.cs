using MyHybridApp2.Application.Common;
using MyHybridApp2.Application.Posts;
using MyHybridApp2.Domain.Posts;
using MyHybridApp2.Shared.Posts;

namespace MyHybridApp2.Application.Tests.Posts;

public sealed class PostCommentsTests
{
    [Fact]
    public async Task CreateAndGetComments_PreservesPostAuthorContentAndTimestamp()
    {
        await using var db = TestDbContext.Create();
        var post = Post.Create("Jane", "A post", null, null, DateTime.UtcNow);
        db.Posts.Add(post);
        await db.SaveChangesAsync(TestContext.Current.CancellationToken);

        var request = new CreatePostCommentRequest(" Alex Johnson ", " A comment ");
        var create = new CreatePostComment(db, new CreatePostCommentRequestValidator(), TimeProvider.System);
        var created = await create.HandleAsync(post.Id, request, TestContext.Current.CancellationToken);

        var comments = await new GetPostComments(db).HandleAsync(post.Id, TestContext.Current.CancellationToken);
        var fetched = await new GetPostCommentById(db).HandleAsync(
            post.Id,
            created.Id,
            TestContext.Current.CancellationToken);

        Assert.Equal(post.Id, created.PostId);
        Assert.Equal("Alex Johnson", created.AuthorName);
        Assert.Equal("A comment", created.Content);
        Assert.NotEqual(default, created.CreatedAt);
        Assert.Equal(created, Assert.Single(comments));
        Assert.Equal(created, fetched);
    }

    [Fact]
    public async Task GetCommentsForUnknownPost_ThrowsNotFound()
    {
        await using var db = TestDbContext.Create();

        await Assert.ThrowsAsync<NotFoundException>(() =>
            new GetPostComments(db).HandleAsync(Guid.NewGuid(), TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task GetPostsAndPostById_IncludeCommentCounts()
    {
        await using var db = TestDbContext.Create();
        var postWithComments = Post.Create("Jane", "A post", null, null, DateTime.UtcNow);
        var postWithoutComments = Post.Create("Sam", "Another post", null, null, DateTime.UtcNow.AddMinutes(-1));
        db.Posts.AddRange(postWithComments, postWithoutComments);
        db.PostComments.AddRange(
            PostComment.Create(postWithComments.Id, "Alex", "First", DateTimeOffset.UtcNow),
            PostComment.Create(postWithComments.Id, "Sam", "Second", DateTimeOffset.UtcNow));
        await db.SaveChangesAsync(TestContext.Current.CancellationToken);

        var posts = await new GetPosts(db).HandleAsync(TestContext.Current.CancellationToken);
        var postFromFeed = Assert.Single(posts, post => post.Id == postWithComments.Id);
        var emptyPostFromFeed = Assert.Single(posts, post => post.Id == postWithoutComments.Id);
        var postById = await new GetPostById(db).HandleAsync(
            postWithComments.Id,
            TestContext.Current.CancellationToken);

        Assert.Equal(2, postFromFeed.CommentCount);
        Assert.Equal(0, emptyPostFromFeed.CommentCount);
        Assert.Equal(2, postById.CommentCount);
    }
}
