using ProjectRepositoryEntity = Work_Experience_Search.Models.ProjectRepository;

namespace Work_Experience_Search.Repositories;

public interface IProjectCodeRepository
{
    Task AddAsync(ProjectRepositoryEntity repository, CancellationToken cancellationToken = default);
    Task RemoveAsync(ProjectRepositoryEntity repository, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
