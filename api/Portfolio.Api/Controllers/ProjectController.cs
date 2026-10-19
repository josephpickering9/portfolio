using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Api.Extensions;
using Portfolio.Api.Models;
using Portfolio.Api.Requests;
using Portfolio.Api.Responses;
using Portfolio.Api.Services;
using Portfolio.Api.Types;

namespace Portfolio.Api.Controllers;

[ApiController]
[Route("[controller]")]
public sealed class ProjectController(IProjectService projectService) : ControllerBase
{
    [HttpGet]
    public Task<ActionResult<IReadOnlyList<ProjectSummary>>> GetProjects([FromQuery] SearchQuery query, CancellationToken cancellationToken) =>
        projectService.GetProjectsAsync(query.Search, cancellationToken).ToResponse();

    [HttpGet("{id:guid}")]
    public Task<ActionResult<Project>> GetProjectById(ProjectId id, CancellationToken cancellationToken) =>
        projectService.GetProjectAsync(id, cancellationToken).ToResponse();

    [HttpGet("{slug}")]
    public Task<ActionResult<Project>> GetProjectBySlug(string slug, CancellationToken cancellationToken) =>
        projectService.GetProjectBySlugAsync(slug, cancellationToken).ToResponse();

    [HttpGet("{id:guid}/related")]
    public Task<ActionResult<IReadOnlyList<Project>>> GetRelatedProjects(ProjectId id, CancellationToken cancellationToken) =>
        projectService.GetRelatedProjectsAsync(id, cancellationToken).ToResponse();

    [HttpPost]
    [Authorize]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public Task<ActionResult<Project>> PostProject([FromForm] CreateProject createProject, CancellationToken cancellationToken) =>
        projectService.CreateProjectAsync(createProject, cancellationToken)
            .ToCreatedResponse(nameof(GetProjectById), project => new { id = project.Id });

    [HttpPut("{id:guid}")]
    [Authorize]
    [Consumes("multipart/form-data")]
    public Task<ActionResult<Project>> PutProject(ProjectId id, [FromForm] CreateProject createProject, CancellationToken cancellationToken) =>
        projectService.UpdateProjectAsync(id, createProject, cancellationToken).ToResponse();

    [HttpDelete("{id:guid}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public Task<IActionResult> DeleteProject(ProjectId id, CancellationToken cancellationToken) =>
        projectService.DeleteProjectAsync(id, cancellationToken).ToNoContentOrError();
}
