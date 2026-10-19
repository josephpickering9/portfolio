using Microsoft.Extensions.Options;
using Work_Experience_Search.Repositories;
using Work_Experience_Search.Types;

namespace Work_Experience_Search.Services.VertexAi;

public interface IVertexQueryService
{
    Task<Result<VertexQueryResult>> QueryAsync(string query, CancellationToken cancellationToken = default);
}

public sealed class VertexQueryService(
    IVertexGenerateContentClient generateContentClient,
    IOptions<VertexAiOptions> options,
    IProjectRepository projectRepository,
    ICompanyRepository companyRepository,
    ITagRepository tagRepository,
    ILogger<VertexQueryService> logger) : IVertexQueryService
{
    public const string FallbackAnswer = "I don't have enough information to answer that.";

    private const string SystemPrompt = """
        You are a portfolio assistant for Joseph Pickering (also known as Joe), a software developer.

        Use only the retrieved context to answer questions. When the answer requires synthesising across multiple retrieved documents, draw connections between them to provide a complete answer.

        Only respond with "I don't have enough information to answer that." if the retrieved context contains no relevant information whatsoever.

        Context notes:
        - Common abbreviations: GCP = Google Cloud Platform, AWS = Amazon Web Services, CI/CD = continuous integration/deployment.
        - Portfolio data is written in the first person; all "I / my / me" statements refer to Joseph Pickering.
        - Queries may use "Joe", "Joseph", or "Joseph Pickering" — treat all as the same person.

        Style:
        - Concise and factual. Reference project titles, company names, or technologies where relevant.
        - UK English.
        """;

    public async Task<Result<VertexQueryResult>> QueryAsync(string query, CancellationToken cancellationToken = default)
    {
        var response = await generateContentClient.GenerateContentAsync(BuildRequest(query), cancellationToken);
        if (!response.IsSuccess) return response.Error!.ToFail();

        var candidate = response.Value!.Candidates.FirstOrDefault();
        var citations = await EnrichCitationsAsync(VertexCitationExtractor.Extract(candidate), cancellationToken);

        var answer = candidate?.Content?.Parts.FirstOrDefault()?.Text;
        if (string.IsNullOrEmpty(answer))
        {
            var finishReasons = response.Value.Candidates.Select(c => c.FinishReason ?? "null");
            logger.LogWarning("Vertex query returned an empty answer. FinishReasons: [{FinishReasons}]", string.Join(", ", finishReasons));
            answer = FallbackAnswer;
        }

        return new VertexQueryResult(answer, citations).ToOk();
    }

    private GenerateContentRequest BuildRequest(string query)
    {
        var datastore = options.Value.GetFeatureDataStoreResource(VertexFeatureType.Project);
        return new GenerateContentRequest(
            GenerateContentMessage.FromSystem(SystemPrompt),
            [GenerateContentMessage.FromUser(query)],
            [new GenerateContentTool(new GenerateContentRetrieval(new VertexAiSearchSource(datastore)))]);
    }

    private async Task<IReadOnlyList<VertexCitation>> EnrichCitationsAsync(IReadOnlyList<RawVertexCitation> citations, CancellationToken cancellationToken)
    {
        var projectIds = IdsOf(citations, VertexFeatureType.Project, id => new ProjectId(id));
        var companyIds = IdsOf(citations, VertexFeatureType.Company, id => new CompanyId(id));
        var tagIds = IdsOf(citations, VertexFeatureType.Tag, id => new TagId(id));

        var projects = (await projectRepository.GetByIdsAsync(projectIds, cancellationToken)).ToDictionary(p => p.Id);
        var companies = (await companyRepository.GetByIdsAsync(companyIds, cancellationToken)).ToDictionary(c => c.Id);
        var tags = (await tagRepository.GetByIdsAsync(tagIds, cancellationToken)).ToDictionary(t => t.Id);

        return citations.Select(citation =>
        {
            var project = citation is { FeatureType: VertexFeatureType.Project, Id: { } projectId } ? projects.GetValueOrDefault(new ProjectId(projectId)) : null;
            var company = citation is { FeatureType: VertexFeatureType.Company, Id: { } companyId } ? companies.GetValueOrDefault(new CompanyId(companyId)) : null;
            var tag = citation is { FeatureType: VertexFeatureType.Tag, Id: { } tagId } ? tags.GetValueOrDefault(new TagId(tagId)) : null;

            return new VertexCitation
            {
                FeatureType = citation.FeatureType,
                Id = citation.Id,
                Title = citation.Title ?? project?.Title ?? company?.Name ?? tag?.Title,
                Project = project,
                Company = company,
                Tag = tag
            };
        }).ToList();
    }

    private static List<TId> IdsOf<TId>(IEnumerable<RawVertexCitation> citations, VertexFeatureType featureType, Func<Guid, TId> toId) =>
        citations
            .Where(c => c.FeatureType == featureType && c.Id.HasValue)
            .Select(c => toId(c.Id!.Value))
            .Distinct()
            .ToList();
}
