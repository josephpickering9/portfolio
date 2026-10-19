using Work_Experience_Search.Services;
using ProjectRepositoryEntity = Work_Experience_Search.Models.ProjectRepository;

namespace Work_Experience_Search.Repositories;

public sealed class ProjectCodeRepository(Database context, CacheInvalidator cacheInvalidator) : IProjectCodeRepository
{
    public async Task AddAsync(ProjectRepositoryEntity repository, CancellationToken cancellationToken = default)
    {
        context.ProjectRepository.Add(repository);
        await SaveChangesAsync(cancellationToken);
    }

    public async Task RemoveAsync(ProjectRepositoryEntity repository, CancellationToken cancellationToken = default)
    {
        context.ProjectRepository.Remove(repository);
        await SaveChangesAsync(cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await context.SaveChangesAsync(cancellationToken);
        cacheInvalidator.InvalidateProjects();
    }
}
