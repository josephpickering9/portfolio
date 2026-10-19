using Portfolio.Api.Models;
using Portfolio.Api.Requests;
using Portfolio.Api.Types;
using ProjectRepositoryEntity = Portfolio.Api.Models.ProjectRepository;

namespace Portfolio.Api.Services;

public interface IProjectRepositoryService
{
    Task<Result<IReadOnlyList<ProjectRepositoryEntity>>> SyncProjectRepositoriesAsync(Project project, IReadOnlyList<CreateProjectRepository> repositories, CancellationToken cancellationToken = default);
}
