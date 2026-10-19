using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Portfolio.Api.Extensions;
using Portfolio.Api.Requests;
using Portfolio.Api.Services.VertexAi;
using Portfolio.Api.Types;

namespace Portfolio.Api.Controllers;

[ApiController]
[Route("[controller]")]
public sealed class VertexController(
    IVertexIngestOrchestrator ingestOrchestrator,
    IVertexQueryService queryService,
    IVertexProjectDescriptionService projectDescriptionService
) : ControllerBase
{
    [HttpPost("ingest")]
    [Authorize]
    public Task<ActionResult<VertexIngestSummary>> Ingest(CancellationToken cancellationToken) =>
        ingestOrchestrator.IngestAllAsync(cancellationToken).ToResponse();

    [HttpPost("query")]
    [EnableRateLimiting(RateLimitPolicies.VertexQuery)]
    public Task<ActionResult<VertexQueryResult>> Query([FromBody] VertexQueryRequest request, CancellationToken cancellationToken) =>
        queryService.QueryAsync(request.Query, cancellationToken).ToResponse();

    [HttpPost("projects/{id:guid}/description/suggest")]
    [Authorize]
    public Task<ActionResult<ProjectDescriptionSuggestionResponse>> SuggestProjectDescription(
        ProjectId id,
        [FromBody] SuggestProjectDescriptionRequest request,
        CancellationToken cancellationToken) =>
        projectDescriptionService.SuggestDescriptionAsync(id, request, cancellationToken).ToResponse();
}
