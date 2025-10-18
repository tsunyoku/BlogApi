namespace BlogApi.Responses;

public class GetUserResponse
{
    public required int Id { get; init; }
    public required string Username { get; init; }
    public required string AvatarUrl { get; init; }
    public required string CountryCode { get; init; }
}
