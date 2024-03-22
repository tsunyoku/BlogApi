using BlogApi.Entities;

namespace BlogApi.Repositories;

public interface IBlogRepository
{
    Task<Blog> AddAsync(
        string title,
        string content,
        CancellationToken cancellationToken = default);

    Task<List<Blog>> GetAsync(CancellationToken cancellationToken = default);

    Task<Blog?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
}