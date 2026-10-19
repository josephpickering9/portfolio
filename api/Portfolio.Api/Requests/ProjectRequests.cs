using Portfolio.Api.Models;
using Portfolio.Api.Types;

namespace Portfolio.Api.Requests;

public sealed record CreateProject
{
    public required string Title { get; init; }
    public required string ShortDescription { get; init; }
    public required string Description { get; init; }
    public string? ProblemStatement { get; init; }
    public string? KeyDecision { get; init; }
    public string? Outcome { get; init; }
    public CompanyId? CompanyId { get; init; }
    public DateOnly StartDate { get; init; }
    public DateOnly? EndDate { get; init; }
    public string? Website { get; init; }
    public bool ShowMockup { get; init; }
    public List<CreateProjectImage> Images { get; init; } = [];
    public List<string> Tags { get; init; } = [];
    public List<CreateProjectRepository> Repositories { get; init; } = [];
    public List<CreateProjectStat> Stats { get; init; } = [];
}

public sealed record CreateProjectImage
{
    public ProjectImageId? Id { get; init; }
    public IFormFile? Image { get; init; }
    public required ImageType Type { get; init; }
    public int? Order { get; init; }
}

public sealed record CreateProjectRepository
{
    public ProjectRepositoryId? Id { get; init; }
    public required string Title { get; init; }
    public required string Url { get; init; }
    public int? Order { get; init; }
}

public sealed record CreateProjectStat
{
    public ProjectStatId? Id { get; init; }
    public required string Label { get; init; }
    public required string Value { get; init; }
    public int? Order { get; init; }
}
