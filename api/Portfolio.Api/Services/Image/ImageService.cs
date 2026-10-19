using TinifyAPI;
using Portfolio.Api.Extensions;
using Portfolio.Api.Types;

namespace Portfolio.Api.Services.Image;

public sealed class ImageService(
    IWebHostEnvironment hostEnvironment,
    IConfiguration configuration,
    ILogger<ImageService> logger) : IImageService
{
    private const string TinifyApiKeySetting = "Tinify:ApiKey";

    private readonly string? _tinifyApiKey = configuration[TinifyApiKeySetting];

    public async Task<Result<ImageData>> GetImageAsync(string fileName, CancellationToken cancellationToken = default)
    {
        var filePath = Path.Combine(hostEnvironment.GetUploadDirectory(), Path.GetFileName(fileName));

        if (!File.Exists(filePath))
        {
            logger.LogWarning("Image {FileName} not found.", fileName);
            return ErrorCodes.FileNotFound.ToFail();
        }

        try
        {
            var file = await File.ReadAllBytesAsync(filePath, cancellationToken);
            return new ImageData(fileName, file, FileExtensions.GetContentType(filePath)).ToOk();
        }
        catch (IOException e)
        {
            logger.LogError(e, "Failed to read image {FileName}.", fileName);
            return ErrorCodes.InternalError.ToFail();
        }
    }

    public async Task<Result<byte[]>> OptimiseImageAsync(byte[] image, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_tinifyApiKey))
            return ErrorCodes.ImageOptimisationUnavailable.ToFail();

        Tinify.Key = _tinifyApiKey;

        try
        {
            var optimised = await Tinify.FromBuffer(image).ToBuffer().WaitAsync(cancellationToken);
            return optimised.ToOk();
        }
        catch (TinifyException e)
        {
            logger.LogError(e, "Tinify image optimisation failed.");
            return ErrorCodes.ImageOptimisationUnavailable.ToFail();
        }
    }
}

public sealed record ImageData(string FileName, byte[] File, string ContentType);
