using System.ComponentModel.DataAnnotations;

namespace BlogApi.Configuration;

public class OsuSettings
{
    [Required]
    public required string ClientId { get; init; }

    [Required]
    public required string ClientSecret { get; init; }
}
