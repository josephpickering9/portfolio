using Work_Experience_Search.Types;

namespace Work_Experience_Search.Services.Image;

public interface IImageService
{
    Task<Result<ImageData>> GetImageAsync(string fileName, CancellationToken cancellationToken = default);
    Task<Result<byte[]>> OptimiseImageAsync(byte[] image, CancellationToken cancellationToken = default);
}
