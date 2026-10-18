using Grpc.Core;
using Work_Experience_Search.Types;

namespace Work_Experience_Search.Services.VertexAi;

public interface IVertexIngestOrchestrator
{
    Task<Result<VertexIngestSummary>> IngestAllAsync(CancellationToken cancellationToken = default);
}

public sealed class VertexIngestOrchestrator(
    IProjectService projectService,
    ICompanyService companyService,
    ITagService tagService,
    IVertexIngestService vertexIngestService,
    IVertexChatbotClient vertexChatbotClient,
    ILogger<VertexIngestOrchestrator> logger) : IVertexIngestOrchestrator
{
    public async Task<Result<VertexIngestSummary>> IngestAllAsync(CancellationToken cancellationToken = default)
    {
        var companies = await companyService.GetCompaniesAsync(null, cancellationToken);
        if (!companies.IsSuccess) return companies.Error!.ToFail();

        var tags = await tagService.GetTagsAsync(null, cancellationToken);
        if (!tags.IsSuccess) return tags.Error!.ToFail();

        var projects = await projectService.GetProjectsAsync(null, cancellationToken);
        if (!projects.IsSuccess) return projects.Error!.ToFail();

        try
        {
            await vertexChatbotClient.InitialiseCachesAsync(ensureSchema: true, cancellationToken);
            await Task.WhenAll(companies.Data!.Select(company => vertexIngestService.UpsertCompanyAsync(company, cancellationToken)));
            await Task.WhenAll(tags.Data!.Select(tag => vertexIngestService.UpsertTagAsync(tag, cancellationToken)));
            await Task.WhenAll(projects.Data!.Select(project => vertexIngestService.UpsertProjectAsync(project, cancellationToken)));
        }
        catch (RpcException ex) when (ex.StatusCode is not StatusCode.Cancelled)
        {
            logger.LogError(ex, "Vertex ingest failed with gRPC status {StatusCode}.", ex.StatusCode);
            return ErrorCodes.UpstreamUnavailable.ToFail();
        }

        return new VertexIngestSummary(
            CompaniesIngested: companies.Data!.Count,
            TagsIngested: tags.Data!.Count,
            ProjectsIngested: projects.Data!.Count).ToOk();
    }
}

public sealed record VertexIngestSummary(int CompaniesIngested, int TagsIngested, int ProjectsIngested);
