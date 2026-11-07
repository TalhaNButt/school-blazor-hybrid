// src/MyHybridApp2.Api/Data/AppDbContext.cs
using Microsoft.EntityFrameworkCore;
using MyHybridApp2.Api.Entities;

namespace MyHybridApp2.Api.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<PostEntity> Posts => Set<PostEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PostEntity>(entity =>
        {
            entity.ToTable("posts");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id)
                  .HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.AuthorName).HasMaxLength(100).IsRequired();
            entity.Property(e => e.ContentText).HasMaxLength(5000);
            entity.HasIndex(e => e.CreatedAt);
        });
    }
}