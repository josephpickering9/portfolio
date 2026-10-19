using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Portfolio.Api.Requests;
using Portfolio.Api.Models;
using ProjectRepository = Portfolio.Api.Repositories.ProjectRepository;
using Portfolio.Api.Repositories;
using Portfolio.Api.Services;
using Portfolio.Api.Services.Image;
using Portfolio.Api.Types;
using Portfolio.Api.Extensions;
using Portfolio.Api.Tests;
using Xunit;

namespace Portfolio.Api.Tests.Unit.Services;

public class ProjectImageServiceTests : BaseServiceTests
{
    private readonly ProjectRepository _projectRepository;
    private readonly ProjectImageService _projectImageService;

    public ProjectImageServiceTests()
    {
        var mockFileService = new Mock<IFileService>();
        var mockImageService = new Mock<IImageService>();
        var cacheInvalidator = new CacheInvalidator();
        _projectRepository = new ProjectRepository(Context, new MemoryCache(new MemoryCacheOptions()), cacheInvalidator);
        _projectImageService = new ProjectImageService(
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
    public async Task SyncProjectImagesAsync_ValidImages_ReturnsProjectImages()
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

        var result = (await _projectImageService.SyncProjectImagesAsync(await GetTrackedProjectAsync(), images)).ExpectSuccess();

        Assert.NotNull(result);
        Assert.Equal(9, result.Count);
    }

    [Fact]
    public async Task SyncProjectImagesAsync_ValidImages_DeletesProjectImages()
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

        await _projectImageService.SyncProjectImagesAsync(await GetTrackedProjectAsync(), images);
        Context.ChangeTracker.Clear();
        Assert.Equal(9, SavedImages().Count);
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

        await _projectImageService.SyncProjectImagesAsync(await GetTrackedProjectAsync(), images);
        Context.ChangeTracker.Clear();
        var result = SavedImages();
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

    private async Task<Project> GetTrackedProjectAsync() =>
        await _projectRepository.GetTrackedAsync(Project1Id) ?? throw new InvalidOperationException("Seeded project missing.");

    private List<ProjectImage> SavedImages() => Context.ProjectImage.Where(image => image.ProjectId == Project1Id).ToList();
}
