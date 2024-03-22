using BlogApi.Abstractions;
using BlogApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace BlogApi;

public class BlogDbContext(DbContextOptions<BlogDbContext> options)
    : DbContext(options), IBlogDbContext
{
    public DbSet<Blog> Blogs { get; set; } = null!;
}