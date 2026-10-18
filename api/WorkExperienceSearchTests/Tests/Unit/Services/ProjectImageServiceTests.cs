using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Work_Experience_Search.Requests;
using Work_Experience_Search.Models;
using ProjectRepository = Work_Experience_Search.Repositories.ProjectRepository;
using Work_Experience_Search.Repositories;
using Work_Experience_Search.Services;
using Work_Experience_Search.Services.Image;
using Work_Experience_Search.Types;
using Work_Experience_Search.Extensions;
using WorkExperienceSearchTests.Tests;
using Xunit;

namespace WorkExperienceSearchTests.Tests.Unit.Services;

public class ProjectImageServiceTests : BaseServiceTests
{
    private readonly ProjectImageService _projectImageService;

    public ProjectImageServiceTests()
    {
        var mockFileService = new Mock<IFileService>();
        var mockImageService = new Mock<IImageService>();
        var cacheInvalidator = new CacheInvalidator();
        _projectImageService = new ProjectImageService(
            new ProjectRepository(Context, new MemoryCache(new MemoryCacheOptions()), cacheInvalidator),
            new ProjectImageRepository(Context, cacheInvalidator),
            mockFileService.Object,
            mockImageService.Object,
            NullLogger<ProjectImageService>.Instance);

        mockFileService.Setup(fs => fs.SaveFileAsync(It.IsAny<IFormFile>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => Result<string>.Ok("testPath"));
        mockImageService.Setup(s => s.OptimiseImageAsync(It.IsAny<byte[]>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => Result<byte[]>.Ok(new byte[12]));
    }

    [Fact]
    public async Task GetProjectImagesAsync_ValidProjectId_ReturnsProjectImages()
    {
        var result = (await _projectImageService.GetProjectImagesAsync(Project1Id)).ExpectSuccess();

        Assert.NotNull(result);
        Assert.Equal(7, result.Count());
    }

    [Fact]
    public async Task GetProjectImagesAsync_InvalidProjectId_ThrowsNotFoundFailure()
    {
        var invalidProjectId = ProjectId.New();

        var result = (await _projectImageService.GetProjectImagesAsync(invalidProjectId)).ExpectFailure();
        Assert.Equal(ErrorCodes.ProjectNotFound, result);
    }

    [Fact]
    public async Task GetProjectImageAsync_ValidProjectIdAndImageId_ReturnsProjectImage()
    {
        var imageId = Image1Id;

        var result = (await _projectImageService.GetProjectImageAsync(Project1Id, imageId)).ExpectSuccess();

        Assert.NotNull(result);
        Assert.Equal(imageId, result.Id);
    }

    [Fact]
    public async Task GetProjectImageAsync_InvalidProjectId_ThrowsNotFoundFailure()
    {
        var invalidProjectId = ProjectId.New();
        var imageId = Image1Id;

        var result = (await _projectImageService.GetProjectImageAsync(invalidProjectId, imageId)).ExpectFailure();
        Assert.Equal(ErrorCodes.ProjectNotFound, result);
    }

    [Fact]
    public async Task GetProjectImageAsync_InvalidImageId_ThrowsNotFoundFailure()
    {
        var invalidImageId = ProjectImageId.New();

        var result = (await _projectImageService.GetProjectImageAsync(Project1Id, invalidImageId)).ExpectFailure();
        Assert.Equal(ErrorCodes.ProjectImageNotFound, result);
    }

    [Fact]
    public async Task SyncProjectImagesAsync_ValidProjectIdAndImages_ReturnsProjectImages()
    {
        var images = new List<CreateProjectImage>
        {
            new() { Id = Image1Id, Type = ImageType.Logo },
            new() { Id = Image2Id, Type = ImageType.Banner },
            new() { Id = Image3Id, Type = ImageType.Card },
            new() { Id = Image4Id, Type = ImageType.Desktop, Order = 1 },
            new() { Id = Image6Id, Type = ImageType.Mobile, Order = 1 },
            new() { Image = null, Type = ImageType.Desktop, Order = 2 },
            new() { Image = null, Type = ImageType.Desktop, Order = 3 },
            new() { Image = null, Type = ImageType.Mobile, Order = 2 },
            new() { Image = null, Type = ImageType.Mobile, Order = 3 }
        };

        var result = (await _projectImageService.SyncProjectImagesAsync(Project1Id, images)).ExpectSuccess();

        Assert.NotNull(result);
        Assert.Equal(9, result.Count);
    }

    [Fact]
    public async Task SyncProjectImagesAsync_ValidProjectIdAndImages_DeletesProjectImages()
    {
        var images = new List<CreateProjectImage>
        {
            new() { Id = Image1Id, Type = ImageType.Logo },
            new() { Id = Image2Id, Type = ImageType.Banner },
            new() { Id = Image3Id, Type = ImageType.Card },
            new() { Id = Image4Id, Type = ImageType.Desktop, Order = 1 },
            new() { Id = Image6Id, Type = ImageType.Mobile, Order = 1 },
            new() { Image = null, Type = ImageType.Desktop, Order = 2 },
            new() { Image = null, Type = ImageType.Desktop, Order = 3 },
            new() { Image = null, Type = ImageType.Mobile, Order = 2 },
            new() { Image = null, Type = ImageType.Mobile, Order = 3 }
        };

        await _projectImageService.SyncProjectImagesAsync(Project1Id, images);
        var result = (await _projectImageService.GetProjectImagesAsync(Project1Id)).ExpectSuccess();

        Assert.NotNull(result);
        Assert.Equal(9, result.Count());
    }

    [Fact]
    public async Task SyncProjectImagesAsync_ReplacingBannerWithNewUpload_LeavesCardUnaffected()
    {
        var images = new List<CreateProjectImage>
        {
            new() { Id = Image1Id, Type = ImageType.Logo },
            new() { Type = ImageType.Banner, Image = null },
            new() { Id = Image3Id, Type = ImageType.Card }
        };

        await _projectImageService.SyncProjectImagesAsync(Project1Id, images);
        var result = (await _projectImageService.GetProjectImagesAsync(Project1Id)).ExpectSuccess();

        var card = result.SingleOrDefault(i => i.Type == ImageType.Card);
        var banner = result.SingleOrDefault(i => i.Type == ImageType.Banner);
        var logo = result.SingleOrDefault(i => i.Type == ImageType.Logo);

        Assert.NotNull(card);
        Assert.Equal("testCard.png", card.Image);
        Assert.NotNull(banner);
        Assert.Equal("testPath", banner.Image);
        Assert.NotNull(logo);
        Assert.Equal("testLogo.png", logo.Image);
    }
}
