using System.ComponentModel.DataAnnotations;

namespace BlogApi.Configuration;

public class OwnerSettings
{
    [Required]
    public required int UserId { get; init; }
}
