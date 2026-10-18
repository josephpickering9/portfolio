using Work_Experience_Search.Models;
using Work_Experience_Search.Requests;
using Work_Experience_Search.Types;
using ProjectRepositoryEntity = Work_Experience_Search.Models.ProjectRepository;

namespace Work_Experience_Search.Services;

public interface IProjectRepositoryService
{
    Task<Result<IReadOnlyList<ProjectRepositoryEntity>>> GetProjectRepositoriesAsync(ProjectId projectId, CancellationToken cancellationToken = default);
    Task<Result<ProjectRepositoryEntity>> GetProjectRepositoryAsync(ProjectId projectId, ProjectRepositoryId id, CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyList<ProjectRepositoryEntity>>> SyncProjectRepositoriesAsync(ProjectId projectId, IReadOnlyList<CreateProjectRepository> repositories, CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyList<ProjectRepositoryEntity>>> SyncProjectRepositoriesAsync(Project project, IReadOnlyList<CreateProjectRepository> repositories, CancellationToken cancellationToken = default);
}
