using Microsoft.EntityFrameworkCore;
using MyHybridApp2.Domain.Posts;

namespace MyHybridApp2.Application.Abstractions;

public interface IAppDbContext
{
    DbSet<Post> Posts { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
