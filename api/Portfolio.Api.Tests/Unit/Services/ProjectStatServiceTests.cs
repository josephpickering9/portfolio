using Microsoft.Extensions.Caching.Memory;
using Portfolio.Api.Models;
using Portfolio.Api.Requests;
using Portfolio.Api.Repositories;
using Portfolio.Api.Services;
using Portfolio.Api.Tests;
using Xunit;
using ProjectRepository = Portfolio.Api.Repositories.ProjectRepository;

namespace Portfolio.Api.Tests.Unit.Services;

public class ProjectStatServiceTests : BaseServiceTests
{
    private readonly ProjectRepository _projectRepository;
    private readonly ProjectStatService _projectStatService;

    public ProjectStatServiceTests()
    {
        var cacheInvalidator = new CacheInvalidator();
        _projectRepository = new ProjectRepository(Context, new MemoryCache(new MemoryCacheOptions()), cacheInvalidator);
        _projectStatService = new ProjectStatService(new ProjectStatRepository(Context, cacheInvalidator));
    }

    [Fact]
    public async Task SyncProjectStatsAsync_NewStats_CreatesStats()
    {
        var stats = new List<CreateProjectStat>
        {
            new() { Label = "gigs logged", Value = "242" },
            new() { Label = "span", Value = "17y", Order = 1 },
        };

        var result = (await _projectStatService.SyncProjectStatsAsync(await GetTrackedProjectAsync(), stats)).ExpectSuccess();

        Assert.Equal(2, result.Count);
        Context.ChangeTracker.Clear();
        Assert.Equal(2, SavedStats().Count);
    }

    [Fact]
    public async Task SyncProjectStatsAsync_RemovingAStat_DeletesIt()
    {
        var initialStats = new List<CreateProjectStat>
        {
            new() { Label = "gigs logged", Value = "242" },
            new() { Label = "span", Value = "17y" },
        };
        var created = (await _projectStatService.SyncProjectStatsAsync(await GetTrackedProjectAsync(), initialStats)).ExpectSuccess();
        var keepId = created.First().Id;

        var updatedStats = new List<CreateProjectStat>
        {
            new() { Id = keepId, Label = "gigs logged", Value = "242" },
        };
        await _projectStatService.SyncProjectStatsAsync(await GetTrackedProjectAsync(), updatedStats);

        Context.ChangeTracker.Clear();
        var saved = SavedStats();
        Assert.Single(saved);
        Assert.Equal(keepId, saved.Single().Id);
    }

    [Fact]
    public async Task SyncProjectStatsAsync_EditingAStat_UpdatesLabelValueAndOrder()
    {
        var created = (await _projectStatService.SyncProjectStatsAsync(await GetTrackedProjectAsync(), [new() { Label = "gigs logged", Value = "242" }])).ExpectSuccess();
        var statId = created.Single().Id;

        var edited = new List<CreateProjectStat> { new() { Id = statId, Label = "shows logged", Value = "300", Order = 2 } };
        await _projectStatService.SyncProjectStatsAsync(await GetTrackedProjectAsync(), edited);

        Context.ChangeTracker.Clear();
        var saved = Context.ProjectStat.Single(stat => stat.Id == statId);
        Assert.Equal("shows logged", saved.Label);
        Assert.Equal("300", saved.Value);
        Assert.Equal(2, saved.Order);
    }

    private async Task<Project> GetTrackedProjectAsync() =>
        await _projectRepository.GetTrackedAsync(Project1Id) ?? throw new InvalidOperationException("Seeded project missing.");

    private List<ProjectStat> SavedStats() => Context.ProjectStat.Where(stat => stat.ProjectId == Project1Id).ToList();
}
