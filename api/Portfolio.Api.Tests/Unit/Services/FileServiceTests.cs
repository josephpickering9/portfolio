using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Portfolio.Api.Services;
using Xunit;

namespace Portfolio.Api.Tests.Unit.Services;

public sealed class FileServiceTests : IDisposable
{
    private const string UploadsFolder = "uploads";

    private readonly string _webRoot = Directory.CreateTempSubdirectory("portfolio-files-").FullName;
    private readonly FileService _fileService;

    public FileServiceTests()
    {
        var environment = new Mock<IWebHostEnvironment>();
        environment.SetupGet(e => e.WebRootPath).Returns(_webRoot);
        _fileService = new FileService(environment.Object, NullLogger<FileService>.Instance);
    }

    public void Dispose() => Directory.Delete(_webRoot, recursive: true);

    [Fact]
    public void DeleteFile_StoredFileName_RemovesTheUpload()
    {
        var upload = CreateUpload("cover.webp");

        _fileService.DeleteFile("cover.webp");

        Assert.False(File.Exists(upload));
    }

    [Fact]
    public void DeleteFile_NameWithAPath_OnlyLooksInTheUploadDirectory()
    {
        var upload = CreateUpload("cover.webp");
        var outside = Path.Combine(_webRoot, "cover.webp");
        File.WriteAllText(outside, "not an upload");

        _fileService.DeleteFile("../cover.webp");

        Assert.False(File.Exists(upload));
        Assert.True(File.Exists(outside));
    }

    [Fact]
    public void DeleteFile_UploadCannotBeDeleted_DoesNotThrow()
    {
        if (OperatingSystem.IsWindows()) return;

        var upload = CreateUpload("locked.webp");
        var directory = Path.GetDirectoryName(upload)!;
        File.SetUnixFileMode(directory, UnixFileMode.UserRead | UnixFileMode.UserExecute);
        try
        {
            _fileService.DeleteFile("locked.webp");

            Assert.True(File.Exists(upload));
        }
        finally
        {
            File.SetUnixFileMode(directory, UnixFileMode.UserRead | UnixFileMode.UserWrite | UnixFileMode.UserExecute);
        }
    }

    [Fact]
    public void DeleteFile_MissingOrEmpty_DoesNothing()
    {
        _fileService.DeleteFile("gone.webp");
        _fileService.DeleteFile(null);
        _fileService.DeleteFile(string.Empty);
    }

    private string CreateUpload(string fileName)
    {
        var directory = Path.Combine(_webRoot, UploadsFolder);
        Directory.CreateDirectory(directory);
        var path = Path.Combine(directory, fileName);
        File.WriteAllText(path, "image");
        return path;
    }
}
