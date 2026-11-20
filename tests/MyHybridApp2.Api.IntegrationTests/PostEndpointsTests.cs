using System.Net;
using System.Net.Http.Json;
using MyHybridApp2.Shared.Posts;

namespace MyHybridApp2.Api.IntegrationTests;

public sealed class PostEndpointsTests(ApiFactory factory) : IClassFixture<ApiFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task Health_ReturnsOk()
    {
        var response = await _client.GetAsync("/health", TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task CreatePost_ThenGetById_RoundTrips()
    {
        var ct = TestContext.Current.CancellationToken;

        var create = await _client.PostAsJsonAsync(
            "/api/posts", new CreatePostRequest("Jane", "Hello #dotnet", null, null), ct);
        Assert.Equal(HttpStatusCode.Created, create.StatusCode);
        var created = await create.Content.ReadFromJsonAsync<PostResponse>(ct);

        var fetched = await _client.GetFromJsonAsync<PostResponse>($"/api/posts/{created!.Id}", ct);

        Assert.Equal("Hello #dotnet", fetched!.ContentText);
        Assert.Equal(PostType.Text, fetched.Type);
    }

    [Fact]
    public async Task Like_IncrementsCounter()
    {
        var ct = TestContext.Current.CancellationToken;
        var create = await _client.PostAsJsonAsync(
            "/api/posts", new CreatePostRequest("Jane", "Like me", null, null), ct);
        var created = await create.Content.ReadFromJsonAsync<PostResponse>(ct);

        var like = await _client.PostAsync($"/api/posts/{created!.Id}/like", content: null, ct);
        var liked = await like.Content.ReadFromJsonAsync<PostResponse>(ct);

        Assert.Equal(1, liked!.LikeCount);
    }

    [Fact]
    public async Task CreatePost_Invalid_ReturnsValidationProblem()
    {
        var response = await _client.PostAsJsonAsync(
            "/api/posts", new CreatePostRequest("", null, null, null), TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task GetById_Unknown_ReturnsNotFound()
    {
        var response = await _client.GetAsync($"/api/posts/{Guid.NewGuid()}", TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task PostComments_CreateAndList_PreservesAuthorContentAndTimestamp()
    {
        var ct = TestContext.Current.CancellationToken;
        var postResponse = await _client.PostAsJsonAsync(
            "/api/posts", new CreatePostRequest("Jane", "Comment on this", null, null), ct);
        var post = await postResponse.Content.ReadFromJsonAsync<PostResponse>(ct);

        var createResponse = await _client.PostAsJsonAsync(
            $"/api/posts/{post!.Id}/comments",
            new CreatePostCommentRequest("Alex Johnson", "A useful comment."),
            ct);

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        Assert.NotNull(createResponse.Headers.Location);
        var created = await createResponse.Content.ReadFromJsonAsync<PostCommentResponse>(ct);
        Assert.NotNull(created);
        Assert.Equal(post.Id, created.PostId);
        Assert.Equal("Alex Johnson", created.AuthorName);
        Assert.Equal("A useful comment.", created.Content);
        Assert.InRange(
            (DateTimeOffset.UtcNow - created.CreatedAt).Duration(),
            TimeSpan.Zero,
            TimeSpan.FromMinutes(1));

        var secondCreateResponse = await _client.PostAsJsonAsync(
            $"/api/posts/{post.Id}/comments",
            new CreatePostCommentRequest("Sam Lee", "A second comment."),
            ct);
        Assert.Equal(HttpStatusCode.Created, secondCreateResponse.StatusCode);
        var secondCreated = await secondCreateResponse.Content.ReadFromJsonAsync<PostCommentResponse>(ct);
        Assert.NotNull(secondCreated);

        var list = await _client.GetFromJsonAsync<List<PostCommentResponse>>(
            $"/api/posts/{post.Id}/comments",
            ct);
        var fetched = await _client.GetFromJsonAsync<PostCommentResponse>(
            $"/api/posts/{post.Id}/comments/{created.Id}",
            ct);
        var postWithComments = await _client.GetFromJsonAsync<PostResponse>($"/api/posts/{post.Id}", ct);
        var feed = await _client.GetFromJsonAsync<List<PostResponse>>("/api/posts", ct);

        Assert.NotNull(list);
        Assert.Equal(2, list.Count);
        Assert.Contains(created, list);
        Assert.Contains(secondCreated, list);
        Assert.Equal(created, fetched);
        Assert.Equal(2, postWithComments!.CommentCount);
        Assert.Equal(2, Assert.Single(feed!, item => item.Id == post.Id).CommentCount);
    }

    [Fact]
    public async Task PostComments_InvalidRequest_ReturnsValidationProblem()
    {
        var ct = TestContext.Current.CancellationToken;
        var postResponse = await _client.PostAsJsonAsync(
            "/api/posts", new CreatePostRequest("Jane", "Comment on this", null, null), ct);
        var post = await postResponse.Content.ReadFromJsonAsync<PostResponse>(ct);

        var response = await _client.PostAsJsonAsync(
            $"/api/posts/{post!.Id}/comments",
            new CreatePostCommentRequest("Alex Johnson", " "),
            ct);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task PostComments_UnknownPost_ReturnsNotFound()
    {
        var response = await _client.GetAsync(
            $"/api/posts/{Guid.NewGuid()}/comments",
            TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
