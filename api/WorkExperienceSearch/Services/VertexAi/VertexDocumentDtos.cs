using Work_Experience_Search.Types;

namespace Work_Experience_Search.Services.VertexAi;

public sealed record VertexProjectDto
{
    [VertexField(retrievable: true)] public ProjectId Id { get; init; }
    [VertexKeyProperty("title")] [VertexField(retrievable: true)] public string Title { get; init; } = string.Empty;
    [VertexField(searchable: true, retrievable: true)] public string ShortDescription { get; init; } = string.Empty;
    [VertexKeyProperty("description")] [VertexField(retrievable: true)] public string Description { get; init; } = string.Empty;
    [VertexField(searchable: true, retrievable: true)] public string? ProblemStatement { get; init; }
    [VertexField(searchable: true, retrievable: true)] public string? KeyDecision { get; init; }
    [VertexField(searchable: true, retrievable: true)] public string? Outcome { get; init; }
    [VertexField(searchable: true, retrievable: true)] public IReadOnlyList<string>? Stats { get; init; } = [];
    [VertexKeyProperty("create_time")] [VertexField(retrievable: true)] public DateOnly StartDate { get; init; }
    [VertexField(indexable: true, retrievable: true)] public DateOnly? EndDate { get; init; }
    [VertexKeyProperty("uri")] [VertexField(retrievable: true)] public string? Website { get; init; }
    [VertexField(indexable: true, retrievable: true)] public CompanyId? CompanyId { get; init; }
    [VertexKeyProperty("category")] [VertexField(retrievable: true)] public string? Company { get; init; }
    [VertexField(searchable: true, indexable: true, retrievable: true)] public IReadOnlyList<string> Tags { get; init; } = [];
    [VertexField(indexable: true, retrievable: true)] public IReadOnlyList<string> TagTypes { get; init; } = [];
}

public sealed record VertexCompanyDto
{
    [VertexField(retrievable: true)] public CompanyId Id { get; init; }
    [VertexKeyProperty("title")] [VertexField(retrievable: true)] public string Name { get; init; } = string.Empty;
    [VertexKeyProperty("description")] [VertexField(retrievable: true)] public string Description { get; init; } = string.Empty;
    [VertexKeyProperty("uri")] [VertexField(retrievable: true)] public string? Website { get; init; }
    [VertexField(indexable: true, retrievable: true)] public DateOnly? StartDate { get; init; }
    [VertexField(indexable: true, retrievable: true)] public DateOnly? EndDate { get; init; }
}

public sealed record VertexTagDto
{
    [VertexKeyProperty("title")] [VertexField(retrievable: true)] public string Title { get; init; } = string.Empty;
    [VertexKeyProperty("category")] [VertexField(retrievable: true)] public string Type { get; init; } = string.Empty;
}
