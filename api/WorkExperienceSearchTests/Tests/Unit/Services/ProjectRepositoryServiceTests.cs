using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Abstractions;
using Work_Experience_Search.Requests;
using Work_Experience_Search.Repositories;
using Work_Experience_Search.Services;
using Work_Experience_Search.Utils;
using Xunit;

namespace WorkExperienceSearchTests.Tests.Unit.Services;

public class ProjectRepositoryServiceTests : BaseServiceTests
{
    private readonly ProjectRepositoryService _projectRepositoryService;

    public ProjectRepositoryServiceTests()
    {
        _projectRepositoryService = new ProjectRepositoryService(
            new Work_Experience_Search.Repositories.ProjectRepository(Context, new MemoryCache(new MemoryCacheOptions()), new CacheInvalidator()),
            new ProjectCodeRepository(Context),
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
}
