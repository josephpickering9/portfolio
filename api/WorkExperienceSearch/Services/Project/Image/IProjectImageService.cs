using Work_Experience_Search.Models;
using Work_Experience_Search.Requests;
using Work_Experience_Search.Types;

namespace Work_Experience_Search.Services;

public interface IProjectImageService
{
    Task<Result<IReadOnlyList<ProjectImage>>> SyncProjectImagesAsync(Project project, IReadOnlyList<CreateProjectImage> images, CancellationToken cancellationToken = default);
    Task<Result<bool>> OptimiseImagesAsync(CancellationToken cancellationToken = default);
}
