namespace BlogApi.Requests;

public class CreateBlogRequest
{
    public required string Title { get; set; }
    public required string Content { get; set; }
}