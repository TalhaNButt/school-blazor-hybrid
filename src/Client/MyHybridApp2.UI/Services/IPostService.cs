using MyHybridApp2.UI.Models;
using MyHybridApp2.Shared.Posts;

namespace MyHybridApp2.UI.Services;

public interface IPostService
{
    Task<List<Post>> GetAllPostsAsync();
    Task<Post?> GetPostByIdAsync(Guid id);
    Task<Post> CreatePostAsync(Post post);
    Task ToggleLikeAsync(Guid postId);
    Task ToggleFavoriteAsync(Guid postId);
    Task<bool> SharePostAsync(Guid postId);
    Task<List<PostCommentResponse>> GetPostCommentsAsync(Guid postId);
    Task<PostCommentResponse> CreatePostCommentAsync(Guid postId, CreatePostCommentRequest request);
}
