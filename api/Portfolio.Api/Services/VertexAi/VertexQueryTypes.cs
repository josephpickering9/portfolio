using Portfolio.Api.Models;

namespace Portfolio.Api.Services.VertexAi;

public sealed record VertexQueryResult(string Answer, IReadOnlyList<VertexCitation> Citations);

// Id is a raw Guid because a citation can point at a project, company or tag; FeatureType is the
// discriminator and EnrichCitationsAsync converts to the matching typed ID before any lookup.
public sealed record VertexCitation
{
    public Guid? Id { get; init; }
    public VertexFeatureType? FeatureType { get; init; }
    public string? Title { get; init; }
    public Project? Project { get; init; }
    public Company? Company { get; init; }
    public Tag? Tag { get; init; }
}

internal sealed record RawVertexCitation
{
    public Guid? Id { get; init; }
    public VertexFeatureType? FeatureType { get; init; }
    public string? Title { get; init; }
}
