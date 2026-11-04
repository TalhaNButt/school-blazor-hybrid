using MyHybridApp2.Models;

namespace MyHybridApp2.Services
{
    public interface IPostService
    {
        Task<List<Post>> GetAllPostsAsync();
        Task<Post?> GetPostByIdAsync(Guid id);
        Task<Post> CreatePostAsync(Post post);
        Task ToggleLikeAsync(Guid postId);
        Task ToggleFavoriteAsync(Guid postId);
        Task<bool> SharePostAsync(Guid postId);
    }

    public class PostService : IPostService
    {
        private readonly List<Post> _posts = [];
        private static Random _random = new();

        public PostService()
        {
            // Initialize with sample data for demonstration
            InitializeSampleData();
        }

        public async Task<List<Post>> GetAllPostsAsync()
        {
            await Task.Delay(10); // Simulate async operation
            return _posts.OrderByDescending(p => p.CreatedAt).ToList();
        }

        public async Task<Post?> GetPostByIdAsync(Guid id)
        {
            await Task.Delay(5);
            return _posts.FirstOrDefault(p => p.Id == id);
        }

        public async Task<Post> CreatePostAsync(Post post)
        {
            await Task.Delay(100); // Simulate network delay
            _posts.Insert(0, post);
            return post;
        }

        public async Task ToggleLikeAsync(Guid postId)
        {
            await Task.Delay(50);
            var post = _posts.FirstOrDefault(p => p.Id == postId);
            if (post != null)
            {
                post.IsLiked = !post.IsLiked;
                post.LikeCount += post.IsLiked ? 1 : -1;
            }
        }

        public async Task ToggleFavoriteAsync(Guid postId)
        {
            await Task.Delay(50);
            var post = _posts.FirstOrDefault(p => p.Id == postId);
            if (post != null)
            {
                post.IsFavorited = !post.IsFavorited;
                post.FavoriteCount += post.IsFavorited ? 1 : -1;
            }
        }

        public async Task<bool> SharePostAsync(Guid postId)
        {
            await Task.Delay(50);
            var post = _posts.FirstOrDefault(p => p.Id == postId);
            if (post != null)
            {
                post.ShareCount++;
                post.IsShared = true;
                return true;
            }
            return false;
        }

        private void InitializeSampleData()
        {
            _posts.AddRange(
            [
                new Post
                {
                    AuthorName = "John Doe",
                    AuthorAvatarUrl = null,
                    CreatedAt = DateTime.Now.AddHours(-2),
                    Type = PostType.Text,
                    ContentText = "Just started learning Blazor Hybrid MAUI! This is amazing for cross-platform development. #dotnet #blazor",
                    LikeCount = 24,
                    ShareCount = 3,
                    FavoriteCount = 8
                },
                new Post
                {
                    AuthorName = "Jane Smith",
                    CreatedAt = DateTime.Now.AddHours(-5),
                    Type = PostType.Image,
                    ContentText = "Beautiful sunset from my hike today!",
                    MediaUrl = "https://picsum.photos/seed/nature/600/400",
                    MediaType = "image/jpeg",
                    LikeCount = 156,
                    ShareCount = 12,
                    FavoriteCount = 45
                },
                new Post
                {
                    AuthorName = "Tech Channel",
                    CreatedAt = DateTime.Now.AddDays(-1),
                    Type = PostType.Video,
                    ContentText = "Check out our latest tutorial on .NET 9 features!",
                    MediaUrl = "https://www.w3schools.com/html/mov_bbb.mp4",
                    MediaType = "video/mp4",
                    LikeCount = 312,
                    ShareCount = 89,
                    FavoriteCount = 156
                },
                new Post
                {
                    AuthorName = "Mike Johnson",
                    CreatedAt = DateTime.Now.AddMinutes(-30),
                    Type = PostType.Text,
                    ContentText = "Has anyone tried the new Lumo AI assistant? Looking for feedback before I switch from other tools.",
                    LikeCount = 7,
                    ShareCount = 1,
                    FavoriteCount = 2
                }
            ]);
        }
    }
}