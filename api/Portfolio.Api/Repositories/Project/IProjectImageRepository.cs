using Portfolio.Api.Models;

namespace Portfolio.Api.Repositories;

public interface IProjectImageRepository
{
    Task AddAsync(ProjectImage image, CancellationToken cancellationToken = default);
    Task UpdateAsync(ProjectImage image, CancellationToken cancellationToken = default);
    Task RemoveAsync(ProjectImage image, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ProjectImage>> GetUnoptimisedImagesTrackedAsync(CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
