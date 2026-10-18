using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Abstractions;
using Work_Experience_Search.Requests;
using Work_Experience_Search.Repositories;
using Work_Experience_Search.Services;
using Work_Experience_Search.Types;
using Work_Experience_Search.Extensions;
using WorkExperienceSearchTests.Tests;
using Xunit;

namespace WorkExperienceSearchTests.Tests.Unit.Services;

public class ProjectStatServiceTests : BaseServiceTests
{
    private readonly ProjectStatService _projectStatService;

    public ProjectStatServiceTests()
    {
        var cacheInvalidator = new CacheInvalidator();
        _projectStatService = new ProjectStatService(
            new ProjectRepository(Context, new MemoryCache(new MemoryCacheOptions()), cacheInvalidator),
            new ProjectStatRepository(Context, cacheInvalidator),
            NullLogger<ProjectStatService>.Instance);
    }

    [Fact]
    public async Task GetProjectStatsAsync_ValidProjectId_ReturnsEmptyByDefault()
    {
        var result = (await _projectStatService.GetProjectStatsAsync(Project1Id)).ExpectSuccess();

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetProjectStatsAsync_InvalidProjectId_ThrowsNotFoundFailure()
    {
        var invalidProjectId = ProjectId.New();

        var result = (await _projectStatService.GetProjectStatsAsync(invalidProjectId)).ExpectFailure();
        Assert.Equal(ErrorCodes.ProjectNotFound, result);
    }

    [Fact]
    public async Task GetProjectStatAsync_InvalidStatId_ThrowsNotFoundFailure()
    {
        var invalidStatId = ProjectStatId.New();

        var result = (await _projectStatService.GetProjectStatAsync(Project1Id, invalidStatId)).ExpectFailure();
        Assert.Equal(ErrorCodes.ProjectStatNotFound, result);
    }

    [Fact]
    public async Task SyncProjectStatsAsync_ValidProjectIdAndStats_CreatesStats()
    {
        var stats = new List<CreateProjectStat>
        {
            new() { Label = "gigs logged", Value = "242" },
            new() { Label = "span", Value = "17y", Order = 1 },
        };

        var result = (await _projectStatService.SyncProjectStatsAsync(Project1Id, stats)).ExpectSuccess();

        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task SyncProjectStatsAsync_RemovingAStat_DeletesIt()
    {
        var initialStats = new List<CreateProjectStat>
        {
            new() { Label = "gigs logged", Value = "242" },
            new() { Label = "span", Value = "17y" },
        };
        var created = (await _projectStatService.SyncProjectStatsAsync(Project1Id, initialStats)).ExpectSuccess();
        var keepId = created.First().Id;

        var updatedStats = new List<CreateProjectStat>
        {
            new() { Id = keepId, Label = "gigs logged", Value = "242" },
        };
        await _projectStatService.SyncProjectStatsAsync(Project1Id, updatedStats);

        var result = (await _projectStatService.GetProjectStatsAsync(Project1Id)).ExpectSuccess();

        Assert.Single(result);
        Assert.Equal(keepId, result.Single().Id);
    }

    [Fact]
    public async Task SyncProjectStatsAsync_EditingAStat_UpdatesLabelValueAndOrder()
    {
        var created = (await _projectStatService.SyncProjectStatsAsync(Project1Id, [new() { Label = "gigs logged", Value = "242" }])).ExpectSuccess();
        var statId = created.Single().Id;

        var edited = new List<CreateProjectStat> { new() { Id = statId, Label = "shows logged", Value = "300", Order = 2 } };
        await _projectStatService.SyncProjectStatsAsync(Project1Id, edited);

        Context.ChangeTracker.Clear();
        var saved = Context.ProjectStat.Single(stat => stat.Id == statId);
        Assert.Equal("shows logged", saved.Label);
        Assert.Equal("300", saved.Value);
        Assert.Equal(2, saved.Order);
    }

    [Fact]
    public async Task SyncProjectStatsAsync_InvalidProjectId_ThrowsNotFoundFailure()
    {
        var invalidProjectId = ProjectId.New();

        var result = (await _projectStatService.SyncProjectStatsAsync(invalidProjectId, [])).ExpectFailure();
        Assert.Equal(ErrorCodes.ProjectNotFound, result);
    }
}
