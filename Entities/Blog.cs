namespace BlogApi.Entities;

public class Blog
{
    public required Guid Id { get; init; }
    public required string Title { get; set; }
    public required string Content { get; set; }
    public required DateTimeOffset PublishedAt { get; init; }
}