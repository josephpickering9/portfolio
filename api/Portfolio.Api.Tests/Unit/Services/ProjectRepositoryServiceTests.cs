using Microsoft.Extensions.Caching.Memory;
using Portfolio.Api.Models;
using Portfolio.Api.Requests;
using Portfolio.Api.Repositories;
using Portfolio.Api.Services;
using Portfolio.Api.Tests;
using Xunit;
using ProjectRepository = Portfolio.Api.Repositories.ProjectRepository;

namespace Portfolio.Api.Tests.Unit.Services;

public class ProjectRepositoryServiceTests : BaseServiceTests
{
    private readonly ProjectRepository _projectRepository;
    private readonly ProjectRepositoryService _projectRepositoryService;

    public ProjectRepositoryServiceTests()
    {
        var cacheInvalidator = new CacheInvalidator();
        _projectRepository = new ProjectRepository(Context, new MemoryCache(new MemoryCacheOptions()), cacheInvalidator);
        _projectRepositoryService = new ProjectRepositoryService(new ProjectCodeRepository(Context, cacheInvalidator));
    }

    [Fact]
    public async Task SyncProjectRepositoriesAsync_NewRepository_CreatesIt()
    {
        var project = await GetTrackedProjectAsync();

        var result = (await _projectRepositoryService.SyncProjectRepositoriesAsync(project, [new() { Title = "API", Url = "https://github.com/a/api" }])).ExpectSuccess();

        Context.ChangeTracker.Clear();
        var saved = Context.ProjectRepository.Single(repository => repository.Id == result.Single().Id);
        Assert.Equal("API", saved.Title);
    }

    [Fact]
    public async Task SyncProjectRepositoriesAsync_EditingARepository_UpdatesTitleUrlAndOrder()
    {
        var created = (await _projectRepositoryService.SyncProjectRepositoriesAsync(await GetTrackedProjectAsync(), [new() { Title = "API", Url = "https://github.com/a/api" }])).ExpectSuccess();
        var repositoryId = created.Single().Id;

        var edited = new List<CreateProjectRepository> { new() { Id = repositoryId, Title = "Backend", Url = "https://github.com/a/backend", Order = 2 } };
        await _projectRepositoryService.SyncProjectRepositoriesAsync(await GetTrackedProjectAsync(), edited);

        Context.ChangeTracker.Clear();
        var saved = Context.ProjectRepository.Single(repository => repository.Id == repositoryId);
        Assert.Equal("Backend", saved.Title);
        Assert.Equal("https://github.com/a/backend", saved.Url);
        Assert.Equal(2, saved.Order);
    }

    private async Task<Project> GetTrackedProjectAsync() =>
        await _projectRepository.GetTrackedAsync(Project1Id) ?? throw new InvalidOperationException("Seeded project missing.");
}
