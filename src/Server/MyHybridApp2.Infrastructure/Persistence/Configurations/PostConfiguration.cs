using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyHybridApp2.Domain.Posts;

namespace MyHybridApp2.Infrastructure.Persistence.Configurations;

internal sealed class PostConfiguration : IEntityTypeConfiguration<Post>
{
    public void Configure(EntityTypeBuilder<Post> builder)
    {
        builder.ToTable("posts");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
        builder.Property(e => e.AuthorName).HasMaxLength(Post.AuthorNameMaxLength).IsRequired();
        builder.Property(e => e.ContentText).HasMaxLength(Post.ContentTextMaxLength);
        builder.HasIndex(e => e.CreatedAt);
    }
}
