using BlogApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace BlogApi;

public class BlogDbContext(DbContextOptions<BlogDbContext> options)
    : DbContext(options)
{
    public DbSet<Blog> Blogs { get; init; }
}
