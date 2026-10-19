using Portfolio.Api.Models;
using Portfolio.Api.Requests;
using Portfolio.Api.Types;

namespace Portfolio.Api.Services;

public interface IProjectImageService
{
    Task<Result<IReadOnlyList<ProjectImage>>> SyncProjectImagesAsync(Project project, IReadOnlyList<CreateProjectImage> images, CancellationToken cancellationToken = default);
    Task<Result> OptimiseImagesAsync(CancellationToken cancellationToken = default);
}
