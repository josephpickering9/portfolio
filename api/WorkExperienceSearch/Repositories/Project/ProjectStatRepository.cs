using Work_Experience_Search.Models;
using Work_Experience_Search.Services;

namespace Work_Experience_Search.Repositories;

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
