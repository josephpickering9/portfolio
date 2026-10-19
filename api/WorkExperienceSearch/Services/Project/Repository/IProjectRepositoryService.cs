using Work_Experience_Search.Models;
using Work_Experience_Search.Requests;
using Work_Experience_Search.Types;
using ProjectRepositoryEntity = Work_Experience_Search.Models.ProjectRepository;

namespace Work_Experience_Search.Services;

public interface IProjectRepositoryService
{
    Task<Result<IReadOnlyList<ProjectRepositoryEntity>>> SyncProjectRepositoriesAsync(Project project, IReadOnlyList<CreateProjectRepository> repositories, CancellationToken cancellationToken = default);
}
