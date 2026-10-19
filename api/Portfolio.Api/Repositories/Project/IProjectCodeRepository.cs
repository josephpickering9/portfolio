using ProjectRepositoryEntity = Portfolio.Api.Models.ProjectRepository;

namespace Portfolio.Api.Repositories;

public interface IProjectCodeRepository
{
    Task AddAsync(ProjectRepositoryEntity repository, CancellationToken cancellationToken = default);
    Task RemoveAsync(ProjectRepositoryEntity repository, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
