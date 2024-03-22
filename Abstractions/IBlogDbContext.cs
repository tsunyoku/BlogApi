using BlogApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace BlogApi.Abstractions;

public interface IBlogDbContext
{
    DbSet<Blog> Blogs { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}