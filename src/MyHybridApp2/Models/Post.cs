namespace MyHybridApp2.Models
{
    public class Post
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string AuthorName { get; set; } = string.Empty;
        public string? AuthorAvatarUrl { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public PostType Type { get; set; }
        public string? ContentText { get; set; }
        public string? MediaUrl { get; set; }
        public string? MediaType { get; set; }
        public int LikeCount { get; set; }
        public int ShareCount { get; set; }
        public int FavoriteCount { get; set; }
        public bool IsLiked { get; set; }
        public bool IsFavorited { get; set; }
        public bool IsShared { get; set; }
    }
}