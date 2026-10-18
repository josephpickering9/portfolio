using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Work_Experience_Search.Extensions;
using Work_Experience_Search.Requests;
using Work_Experience_Search.Services.VertexAi;
using Work_Experience_Search.Types;

namespace Work_Experience_Search.Controllers;

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
    public async Task<IActionResult> Ingest(CancellationToken cancellationToken) =>
        (await ingestOrchestrator.IngestAllAsync(cancellationToken)).ToResponse();

    [HttpPost("query")]
    [EnableRateLimiting(RateLimitPolicies.VertexQuery)]
    public async Task<ActionResult<VertexQueryResult>> Query([FromBody] VertexQueryRequest request, CancellationToken cancellationToken) =>
        (await queryService.QueryAsync(request.Query, cancellationToken)).ToResponse();

    [HttpPost("projects/{id:guid}/description/suggest")]
    [Authorize]
    public async Task<ActionResult<ProjectDescriptionSuggestionResponse>> SuggestProjectDescription(
        ProjectId id,
        [FromBody] SuggestProjectDescriptionRequest request,
        CancellationToken cancellationToken) =>
        (await projectDescriptionService.SuggestDescriptionAsync(id, request, cancellationToken)).ToResponse();
}
