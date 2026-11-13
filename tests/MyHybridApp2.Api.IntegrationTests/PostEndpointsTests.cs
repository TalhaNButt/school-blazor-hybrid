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
}
