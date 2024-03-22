using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace BlogApi;

public class BlogDbContextFactory : IDesignTimeDbContextFactory<BlogDbContext>
{
    public BlogDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<BlogDbContext>();
        optionsBuilder
            .UseNpgsql()
            .UseSnakeCaseNamingConvention();

        return new BlogDbContext(optionsBuilder.Options);
    }
}