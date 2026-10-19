using Portfolio.Api.Types;

namespace Portfolio.Api.Services.Image;

public interface IImageService
{
    Task<Result<ImageData>> GetImageAsync(string fileName, CancellationToken cancellationToken = default);
    Task<Result<byte[]>> OptimiseImageAsync(byte[] image, CancellationToken cancellationToken = default);
}
