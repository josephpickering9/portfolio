using Microsoft.EntityFrameworkCore;
using Portfolio.Api.Models;
using Portfolio.Api.Services;

namespace Portfolio.Api.Repositories;

public sealed class ProjectImageRepository(Database context, CacheInvalidator cacheInvalidator) : IProjectImageRepository
{
    public async Task AddAsync(ProjectImage image, CancellationToken cancellationToken = default)
    {
        context.ProjectImage.Add(image);
        await SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(ProjectImage image, CancellationToken cancellationToken = default)
    {
        context.ProjectImage.Update(image);
        await SaveChangesAsync(cancellationToken);
    }

    public async Task RemoveAsync(ProjectImage image, CancellationToken cancellationToken = default)
    {
        context.ProjectImage.Remove(image);
        await SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ProjectImage>> GetUnoptimisedImagesTrackedAsync(CancellationToken cancellationToken = default)
    {
        return await context.ProjectImage.Where(i => !i.IsOptimised).ToListAsync(cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await context.SaveChangesAsync(cancellationToken);
        cacheInvalidator.InvalidateProjects();
    }
}
