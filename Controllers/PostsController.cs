using BlogApi.Entities;
using BlogApi.Repositories;
using BlogApi.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace BlogApi.Controllers;

[Route("posts")]
[ApiController]
public class PostsController(IBlogRepository blogRepository) : ControllerBase
{
    [HttpGet]
    public async Task<Results<Ok<List<Blog>>, ProblemHttpResult>> GetBlogs(CancellationToken cancellationToken)
    {
        return TypedResults.Ok(await blogRepository.GetAsync(cancellationToken));
    }

    [HttpPost]
    [Authorize(Policy = "Owner")]
    public async Task<Results<CreatedAtRoute<Blog>, ProblemHttpResult>> CreateBlog(
        [FromBody] CreateBlogRequest request,
        CancellationToken cancellationToken)
    {
        var blog = await blogRepository.AddAsync(request.Title, request.Content, cancellationToken);
        return TypedResults.CreatedAtRoute(blog, nameof(FindBlogById), new { postId = blog.Id });
    }

    [HttpGet("{postId:guid}", Name = nameof(FindBlogById))]
    public async Task<Results<Ok<Blog>, NotFound, ProblemHttpResult>> FindBlogById(
        [FromRoute] Guid postId,
        CancellationToken cancellationToken)
    {
        var blog = await blogRepository.FindByIdAsync(postId, cancellationToken);
        return blog is null ? TypedResults.NotFound() : TypedResults.Ok(blog);
    }

    [HttpDelete("{postId:guid}")]
    [Authorize(Policy = "Owner")]
    public async Task<Results<NoContent, ProblemHttpResult>> DeleteBlog(
        [FromRoute] Guid postId,
        CancellationToken cancellationToken)
    {
        await blogRepository.DeleteAsync(postId, cancellationToken);
        return TypedResults.NoContent();
    }
}