using Portfolio.Api.Models;

namespace Portfolio.Api.Requests;

public sealed record CreateTag
{
    public required string Title { get; init; }
    public required TagType Type { get; init; }
    public string? Icon { get; init; }
    public string? CustomColour { get; init; }
}
