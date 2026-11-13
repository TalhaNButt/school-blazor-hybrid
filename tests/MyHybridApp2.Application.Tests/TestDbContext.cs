using Microsoft.EntityFrameworkCore;
using MyHybridApp2.Application.Abstractions;
using MyHybridApp2.Domain.Posts;

namespace MyHybridApp2.Application.Tests;

/// <summary>
/// In-memory <see cref="IAppDbContext"/> so Application tests stay independent of Infrastructure.
/// </summary>
internal sealed class TestDbContext(DbContextOptions<TestDbContext> options) : DbContext(options), IAppDbContext
{
    public DbSet<Post> Posts => Set<Post>();

    public static TestDbContext Create() =>
        new(new DbContextOptionsBuilder<TestDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
}
