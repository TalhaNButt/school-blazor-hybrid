using Microsoft.EntityFrameworkCore;
using MyHybridApp2.Application.Abstractions;
using MyHybridApp2.Domain.Posts;

namespace MyHybridApp2.Infrastructure.Persistence;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options), IAppDbContext
{
    public DbSet<Post> Posts => Set<Post>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
