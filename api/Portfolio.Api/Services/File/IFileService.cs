using Portfolio.Api.Types;

namespace Portfolio.Api.Services;

public interface IFileService
{
    Task<Result<string>> SaveFileAsync(IFormFile? file, CancellationToken cancellationToken = default);
    void DeleteFile(string? filePath);
}
