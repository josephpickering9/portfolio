using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Work_Experience_Search.Extensions;
using Work_Experience_Search.Models;
using Work_Experience_Search.Requests;
using Work_Experience_Search.Services;
using Work_Experience_Search.Types;

namespace Work_Experience_Search.Controllers;

[ApiController]
[Route("[controller]")]
public sealed class ProjectController(IProjectService projectService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<Project>>> GetProjects(string? search, CancellationToken cancellationToken) =>
        (await projectService.GetProjectsAsync(search, cancellationToken)).ToResponse();

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Project>> GetProject(ProjectId id, CancellationToken cancellationToken) =>
        (await projectService.GetProjectAsync(id, cancellationToken)).ToResponse();

    [HttpGet("{slug}")]
    public async Task<ActionResult<Project>> GetProject(string slug, CancellationToken cancellationToken) =>
        (await projectService.GetProjectBySlugAsync(slug, cancellationToken)).ToResponse();

    [HttpGet("{id:guid}/related")]
    public async Task<ActionResult<IReadOnlyList<Project>>> GetRelatedProjects(ProjectId id, CancellationToken cancellationToken) =>
        (await projectService.GetRelatedProjectsAsync(id, cancellationToken)).ToResponse();

    [HttpPost]
    [Authorize]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<Project>> PostProject([FromForm] CreateProject createProject, CancellationToken cancellationToken) =>
        (await projectService.CreateProjectAsync(createProject, cancellationToken)).ToResponse();

    [HttpPut("{id:guid}")]
    [Authorize]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<Project>> PutProject(ProjectId id, [FromForm] CreateProject createProject, CancellationToken cancellationToken) =>
        (await projectService.UpdateProjectAsync(id, createProject, cancellationToken)).ToResponse();

    [HttpDelete("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> DeleteProject(ProjectId id, CancellationToken cancellationToken) =>
        (await projectService.DeleteProjectAsync(id, cancellationToken)).ToResponse();
}
