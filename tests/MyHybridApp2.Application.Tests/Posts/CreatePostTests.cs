using FluentValidation;
using MyHybridApp2.Application.Posts;
using MyHybridApp2.Shared.Posts;

namespace MyHybridApp2.Application.Tests.Posts;

public sealed class CreatePostTests
{
    private readonly CreatePostRequestValidator _validator = new();

    [Fact]
    public async Task HandleAsync_ValidRequest_PersistsPostAndMapsType()
    {
        await using var db = TestDbContext.Create();
        var handler = new CreatePost(db, _validator, TimeProvider.System);

        var result = await handler.HandleAsync(
            new CreatePostRequest("Jane", "Sunset", "https://example.com/a.jpg", "image/jpeg"),
            TestContext.Current.CancellationToken);

        Assert.Equal(PostType.Image, result.Type);
        Assert.Equal("Jane", result.AuthorName);
        Assert.Single(db.Posts);
    }

    [Fact]
    public async Task HandleAsync_InvalidRequest_ThrowsValidationException()
    {
        await using var db = TestDbContext.Create();
        var handler = new CreatePost(db, _validator, TimeProvider.System);

        await Assert.ThrowsAsync<ValidationException>(() =>
            handler.HandleAsync(new CreatePostRequest("", null, null, null), TestContext.Current.CancellationToken));
        Assert.Empty(db.Posts);
    }

    [Theory]
    [InlineData("", "text", null, null)]
    [InlineData("Jane", null, null, null)]
    [InlineData("Jane", null, "not-a-url", "image/jpeg")]
    [InlineData("Jane", null, "https://example.com/a.jpg", null)]
    public void Validator_RejectsInvalidRequests(string author, string? text, string? mediaUrl, string? mediaType)
    {
        var result = _validator.Validate(new CreatePostRequest(author, text, mediaUrl, mediaType));

        Assert.False(result.IsValid);
    }

    [Fact]
    public void Validator_AcceptsTextOnlyPost()
    {
        var result = _validator.Validate(new CreatePostRequest("Jane", "Hello #dotnet", null, null));

        Assert.True(result.IsValid);
    }
}
