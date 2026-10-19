using Grpc.Core;
using Portfolio.Api.Repositories;
using Portfolio.Api.Types;

namespace Portfolio.Api.Services.VertexAi;

public interface IVertexIngestOrchestrator
{
    Task<Result<VertexIngestSummary>> IngestAllAsync(CancellationToken cancellationToken = default);
}

public sealed class VertexIngestOrchestrator(
    IProjectRepository projectRepository,
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

        var projects = await projectRepository.SearchAsync(null, cancellationToken);

        try
        {
            await vertexChatbotClient.InitialiseCachesAsync(ensureSchema: true, cancellationToken);
            await Task.WhenAll(companies.Value!.Select(company => vertexIngestService.UpsertCompanyAsync(company, cancellationToken)));
            await Task.WhenAll(tags.Value!.Select(tag => vertexIngestService.UpsertTagAsync(tag, cancellationToken)));
            await Task.WhenAll(projects.Select(project => vertexIngestService.UpsertProjectAsync(project, cancellationToken)));
        }
        catch (RpcException ex) when (ex.StatusCode is not StatusCode.Cancelled)
        {
            logger.LogError(ex, "Vertex ingest failed with gRPC status {StatusCode}.", ex.StatusCode);
            return ErrorCodes.UpstreamUnavailable.ToFail();
        }

        return new VertexIngestSummary(
            CompaniesIngested: companies.Value!.Count,
            TagsIngested: tags.Value!.Count,
            ProjectsIngested: projects.Count).ToOk();
    }
}

public sealed record VertexIngestSummary(int CompaniesIngested, int TagsIngested, int ProjectsIngested);
