using MyHybridApp2.UI.Models;

namespace MyHybridApp2.UI.Services;

public interface IPostService
{
    Task<List<Post>> GetAllPostsAsync();
    Task<Post?> GetPostByIdAsync(Guid id);
    Task<Post> CreatePostAsync(Post post);
    Task ToggleLikeAsync(Guid postId);
    Task ToggleFavoriteAsync(Guid postId);
    Task<bool> SharePostAsync(Guid postId);
}
