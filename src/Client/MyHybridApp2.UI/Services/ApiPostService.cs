using System.Net;
using System.Net.Http.Json;
using MyHybridApp2.Shared.Posts;
using MyHybridApp2.UI.Models;

namespace MyHybridApp2.UI.Services;

/// <summary>
/// Calls the MyHybridApp2 API. Which posts this user liked/favorited is tracked locally until authentication exists.
/// </summary>
public sealed class ApiPostService(HttpClient http) : IPostService
{
    private const string BasePath = "api/posts";

    private readonly HashSet<Guid> _liked = [];
    private readonly HashSet<Guid> _favorited = [];
    private readonly HashSet<Guid> _shared = [];

    public async Task<List<Post>> GetAllPostsAsync()
    {
        var posts = await http.GetFromJsonAsync<List<PostResponse>>(BasePath) ?? [];
        return posts.Select(ToModel).ToList();
    }

    public async Task<Post?> GetPostByIdAsync(Guid id)
    {
        using var response = await http.GetAsync($"{BasePath}/{id}");
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();
        var post = await response.Content.ReadFromJsonAsync<PostResponse>();
        return post is null ? null : ToModel(post);
    }

    public async Task<Post> CreatePostAsync(Post post)
    {
        var request = new CreatePostRequest(post.AuthorName, post.ContentText, post.MediaUrl, post.MediaType);
        using var response = await http.PostAsJsonAsync(BasePath, request);
        response.EnsureSuccessStatusCode();

        var created = await response.Content.ReadFromJsonAsync<PostResponse>()
            ?? throw new InvalidOperationException("The API returned an empty response.");

        var model = ToModel(created);
        model.AuthorAvatarUrl = post.AuthorAvatarUrl;
        return model;
    }

    public Task ToggleLikeAsync(Guid postId) => ToggleAsync(postId, _liked, "like");

    public Task ToggleFavoriteAsync(Guid postId) => ToggleAsync(postId, _favorited, "favorite");

    public async Task<bool> SharePostAsync(Guid postId)
    {
        using var response = await http.PostAsync($"{BasePath}/{postId}/share", content: null);
        if (!response.IsSuccessStatusCode)
        {
            return false;
        }

        _shared.Add(postId);
        return true;
    }

    private async Task ToggleAsync(Guid postId, HashSet<Guid> state, string action)
    {
        var isOn = state.Contains(postId);
        var uri = $"{BasePath}/{postId}/{action}";

        using var response = isOn
            ? await http.DeleteAsync(uri)
            : await http.PostAsync(uri, content: null);
        response.EnsureSuccessStatusCode();

        if (isOn)
        {
            state.Remove(postId);
        }
        else
        {
            state.Add(postId);
        }
    }

    private Post ToModel(PostResponse response) => new()
    {
        Id = response.Id,
        AuthorName = response.AuthorName,
        CreatedAt = response.CreatedAt.ToLocalTime(),
        Type = response.Type,
        ContentText = response.ContentText,
        MediaUrl = response.MediaUrl,
        MediaType = response.MediaType,
        LikeCount = response.LikeCount,
        ShareCount = response.ShareCount,
        FavoriteCount = response.FavoriteCount,
        IsLiked = _liked.Contains(response.Id),
        IsFavorited = _favorited.Contains(response.Id),
        IsShared = _shared.Contains(response.Id)
    };
}
