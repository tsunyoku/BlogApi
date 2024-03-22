using BlogApi.Abstractions;
using BlogApi.Entities;
using BlogApi.Repositories;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace BlogApi.Controllers;

[Route("api/posts")]
[ApiController]
public class PostsController(IBlogRepository blogRepository, IBlogDbContext dbContext) : ControllerBase
{
    [HttpGet]
    public async Task<Results<Ok<List<Blog>>, ProblemHttpResult>> GetBlogs(CancellationToken cancellationToken)
    {
        return TypedResults.Ok(await blogRepository.GetAsync(cancellationToken));
    }

    [HttpGet("{postId:guid}")]
    public async Task<Results<Ok<Blog>, NotFound, ProblemHttpResult>> FindBlogById(
        [FromRoute] Guid postId,
        CancellationToken cancellationToken)
    {
        var blog = await blogRepository.FindByIdAsync(postId, cancellationToken);
        return blog is null ? TypedResults.NotFound() : TypedResults.Ok(blog);
    }
}