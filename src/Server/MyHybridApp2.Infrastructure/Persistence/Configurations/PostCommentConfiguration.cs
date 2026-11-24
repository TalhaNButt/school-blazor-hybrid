using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyHybridApp2.Domain.Posts;

namespace MyHybridApp2.Infrastructure.Persistence.Configurations;

internal sealed class PostCommentConfiguration : IEntityTypeConfiguration<PostComment>
{
    public void Configure(EntityTypeBuilder<PostComment> builder)
    {
        builder.ToTable("post_comments");
        builder.HasKey(comment => comment.Id);
        builder.Property(comment => comment.Id).HasDefaultValueSql("gen_random_uuid()");
        builder.Property(comment => comment.AuthorName).HasMaxLength(PostComment.AuthorNameMaxLength).IsRequired();
        builder.Property(comment => comment.Content).HasMaxLength(PostComment.ContentMaxLength).IsRequired();
        builder.Property(comment => comment.CreatedAt).IsRequired();
        builder.HasOne<Post>()
            .WithMany()
            .HasForeignKey(comment => comment.PostId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasIndex(comment => new { comment.PostId, comment.CreatedAt });
    }
}
