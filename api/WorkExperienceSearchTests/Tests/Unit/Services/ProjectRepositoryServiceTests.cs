using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Abstractions;
using Work_Experience_Search.Requests;
using Work_Experience_Search.Repositories;
using Work_Experience_Search.Services;
using Work_Experience_Search.Extensions;
using WorkExperienceSearchTests.Tests;
using Work_Experience_Search.Types;
using Xunit;

namespace WorkExperienceSearchTests.Tests.Unit.Services;

public class ProjectRepositoryServiceTests : BaseServiceTests
{
    private readonly ProjectRepositoryService _projectRepositoryService;

    public ProjectRepositoryServiceTests()
    {
        var cacheInvalidator = new CacheInvalidator();
        _projectRepositoryService = new ProjectRepositoryService(
            new ProjectRepository(Context, new MemoryCache(new MemoryCacheOptions()), cacheInvalidator),
            new ProjectCodeRepository(Context, cacheInvalidator),
            NullLogger<ProjectRepositoryService>.Instance);
    }

    [Fact]
    public async Task SyncProjectRepositoriesAsync_EditingARepository_UpdatesTitleUrlAndOrder()
    {
        var created = (await _projectRepositoryService.SyncProjectRepositoriesAsync(Project1Id, [new() { Title = "API", Url = "https://github.com/a/api" }])).ExpectSuccess();
        var repositoryId = created.Single().Id;

        var edited = new List<CreateProjectRepository> { new() { Id = repositoryId, Title = "Backend", Url = "https://github.com/a/backend", Order = 2 } };
        await _projectRepositoryService.SyncProjectRepositoriesAsync(Project1Id, edited);

        Context.ChangeTracker.Clear();
        var saved = Context.ProjectRepository.Single(repository => repository.Id == repositoryId);
        Assert.Equal("Backend", saved.Title);
        Assert.Equal("https://github.com/a/backend", saved.Url);
        Assert.Equal(2, saved.Order);
    }

    [Fact]
    public async Task GetProjectRepositoryAsync_ExistingRepository_ReturnsRepository()
    {
        var created = (await _projectRepositoryService.SyncProjectRepositoriesAsync(Project1Id, [new() { Title = "API", Url = "https://github.com/a/api" }])).ExpectSuccess();

        var result = (await _projectRepositoryService.GetProjectRepositoryAsync(Project1Id, created.Single().Id)).ExpectSuccess();

        Assert.Equal("API", result.Title);
    }

    [Fact]
    public async Task GetProjectRepositoryAsync_UnknownRepository_ReturnsProjectRepositoryNotFound()
    {
        var error = (await _projectRepositoryService.GetProjectRepositoryAsync(Project1Id, ProjectRepositoryId.New())).ExpectFailure();

        Assert.Equal(ErrorCodes.ProjectRepositoryNotFound, error);
    }

    [Fact]
    public async Task SyncProjectRepositoriesAsync_UnknownProject_ReturnsProjectNotFound()
    {
        var error = (await _projectRepositoryService.SyncProjectRepositoriesAsync(ProjectId.New(), [])).ExpectFailure();

        Assert.Equal(ErrorCodes.ProjectNotFound, error);
    }
}
