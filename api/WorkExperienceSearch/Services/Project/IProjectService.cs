using Work_Experience_Search.Models;
using Work_Experience_Search.Requests;
using Work_Experience_Search.Types;

namespace Work_Experience_Search.Services;

public interface IProjectService
{
    Task<Result<IReadOnlyList<Project>>> GetProjectsAsync(string? search, CancellationToken cancellationToken = default);
    Task<Result<Project>> GetProjectAsync(ProjectId id, CancellationToken cancellationToken = default);
    Task<Result<Project>> GetProjectBySlugAsync(string slug, CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyList<Project>>> GetRelatedProjectsAsync(ProjectId projectId, CancellationToken cancellationToken = default);
    Task<Result<Project>> CreateProjectAsync(CreateProject createProject, CancellationToken cancellationToken = default);
    Task<Result<Project>> UpdateProjectAsync(ProjectId id, CreateProject createProject, CancellationToken cancellationToken = default);
    Task<Result<Project>> DeleteProjectAsync(ProjectId id, CancellationToken cancellationToken = default);
}
