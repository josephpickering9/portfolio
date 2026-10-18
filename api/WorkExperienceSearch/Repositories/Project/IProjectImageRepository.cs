using Work_Experience_Search.Models;

namespace Work_Experience_Search.Repositories;

public interface IProjectImageRepository
{
    Task AddAsync(ProjectImage image, CancellationToken cancellationToken = default);
    Task UpdateAsync(ProjectImage image, CancellationToken cancellationToken = default);
    Task RemoveAsync(ProjectImage image, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ProjectImage>> GetUnoptimisedImagesTrackedAsync(CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
