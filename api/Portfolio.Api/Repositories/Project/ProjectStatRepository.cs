using Portfolio.Api.Models;
using Portfolio.Api.Services;

namespace Portfolio.Api.Repositories;

public sealed class ProjectStatRepository(Database context, CacheInvalidator cacheInvalidator) : IProjectStatRepository
{
    public async Task AddAsync(ProjectStat stat, CancellationToken cancellationToken = default)
    {
        context.ProjectStat.Add(stat);
        await SaveChangesAsync(cancellationToken);
    }

    public async Task RemoveAsync(ProjectStat stat, CancellationToken cancellationToken = default)
    {
        context.ProjectStat.Remove(stat);
        await SaveChangesAsync(cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await context.SaveChangesAsync(cancellationToken);
        cacheInvalidator.InvalidateProjects();
    }
}
