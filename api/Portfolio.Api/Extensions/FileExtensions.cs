using Microsoft.AspNetCore.StaticFiles;

namespace Portfolio.Api.Extensions;

public static class FileExtensions
{
    public const string DefaultContentType = "application/octet-stream";
    public const string DefaultFileName = "image.png";
    private const string UploadsFolder = "uploads";

    public static async Task<byte[]> ReadAllBytesAsync(this IFormFile? file, CancellationToken cancellationToken = default)
    {
        if (file is null) return [];

        await using var stream = file.OpenReadStream();
        using var buffer = new MemoryStream();
        await stream.CopyToAsync(buffer, cancellationToken);
        return buffer.ToArray();
    }

    public static IFormFile ToFormFile(this byte[] fileBytes, string fileName, string contentType)
    {
        var stream = new MemoryStream(fileBytes);
        return new FormFile(stream, 0, stream.Length, fileName, fileName)
        {
            Headers = new HeaderDictionary(),
            ContentType = contentType,
        };
    }

    public static string GetContentType(string fileName) =>
        new FileExtensionContentTypeProvider().TryGetContentType(fileName, out var contentType)
            ? contentType
            : DefaultContentType;

    public static string GetUploadDirectory(this IWebHostEnvironment environment) =>
        Path.Combine(environment.WebRootPath ?? environment.ContentRootPath, UploadsFolder);
}
