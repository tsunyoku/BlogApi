using BlogApi.Abstractions;
using BlogApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace BlogApi.Repositories;

public class BlogRepository(IBlogDbContext dbContext) : IBlogRepository
{
    public async Task<Blog> AddAsync(
        string title,
        string content,
        CancellationToken cancellationToken = default)
    {
        var blog = new Blog
        {
            Id = Guid.NewGuid(),
            Title = title,
            Content = content,
            PublishedAt = DateTimeOffset.UtcNow,
        };

        await dbContext.Blogs.AddAsync(blog, cancellationToken);
        return blog;
    }

    public Task<List<Blog>> GetAsync(CancellationToken cancellationToken = default)
    {
        return dbContext.Blogs.ToListAsync(cancellationToken);
    }

    public Task<Blog?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return dbContext.Blogs.FirstOrDefaultAsync(b => b.Id == id, cancellationToken);
    }
}