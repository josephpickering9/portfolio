using Portfolio.Api.Models;
using Portfolio.Api.Requests;
using Portfolio.Api.Responses;
using Portfolio.Api.Types;

namespace Portfolio.Api.Services;

public interface IProjectService
{
    Task<Result<IReadOnlyList<ProjectSummary>>> GetProjectsAsync(string? search, CancellationToken cancellationToken = default);
    Task<Result<Project>> GetProjectAsync(ProjectId id, CancellationToken cancellationToken = default);
    Task<Result<Project>> GetProjectBySlugAsync(string slug, CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyList<Project>>> GetRelatedProjectsAsync(ProjectId projectId, CancellationToken cancellationToken = default);
    Task<Result<Project>> CreateProjectAsync(CreateProject createProject, CancellationToken cancellationToken = default);
    Task<Result<Project>> UpdateProjectAsync(ProjectId id, CreateProject createProject, CancellationToken cancellationToken = default);
    Task<Result> DeleteProjectAsync(ProjectId id, CancellationToken cancellationToken = default);
}
