using Portfolio.Api.Extensions;
using Portfolio.Api.Types;

namespace Portfolio.Api.Services;

public sealed class FileService(IWebHostEnvironment hostEnvironment, ILogger<FileService> logger) : IFileService
{
    public async Task<Result<string>> SaveFileAsync(IFormFile? file, CancellationToken cancellationToken = default)
    {
        if (file is not { Length: > 0 })
        {
            logger.LogWarning("File save rejected: no file uploaded for {FileName}.", file?.FileName);
            return ErrorCodes.FileRequired.ToFail();
        }

        var uploadDirectory = hostEnvironment.GetUploadDirectory();
        Directory.CreateDirectory(uploadDirectory);

        var filePath = Path.Combine(uploadDirectory, Guid.NewGuid() + Path.GetExtension(file.FileName));

        await using var fileStream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(fileStream, cancellationToken);

        return filePath.ToOk();
    }

    public void DeleteFile(string? fileName)
    {
        if (string.IsNullOrEmpty(fileName)) return;

        var filePath = Path.Combine(hostEnvironment.GetUploadDirectory(), Path.GetFileName(fileName));
        try
        {
            if (File.Exists(filePath)) File.Delete(filePath);
        }
        catch (Exception e) when (e is IOException or UnauthorizedAccessException)
        {
            logger.LogWarning(e, "Could not delete uploaded file {FileName}.", fileName);
        }
    }
}
