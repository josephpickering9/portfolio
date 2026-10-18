using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Work_Experience_Search.Requests;
using Work_Experience_Search.Models;
using Work_Experience_Search.Repositories;
using ProjectRepositoryEntity = Work_Experience_Search.Models.ProjectRepository;
using ProjectRepository = Work_Experience_Search.Repositories.ProjectRepository;
using Work_Experience_Search.Services;
using Work_Experience_Search.Types;
using Work_Experience_Search.Extensions;
using WorkExperienceSearchTests.Tests;
using Xunit;

namespace WorkExperienceSearchTests.Tests.Unit.Services;

public class ProjectServiceTests : BaseServiceTests
{
    private readonly Mock<ITagService> _mockTagService;
    private readonly Mock<IProjectImageService> _mockProjectImageService;
    private readonly Mock<IProjectRepositoryService> _mockProjectRepositoryService;
    private readonly Mock<IProjectStatService> _mockProjectStatService;
    private readonly IProjectService _projectService;

    public ProjectServiceTests()
    {
        _mockTagService = new Mock<ITagService>();
        _mockProjectImageService = new Mock<IProjectImageService>();
        _mockProjectRepositoryService = new Mock<IProjectRepositoryService>();
        _mockProjectStatService = new Mock<IProjectStatService>();
        var projectRepository = new ProjectRepository(Context, new MemoryCache(new MemoryCacheOptions()), new CacheInvalidator());
        _projectService = new ProjectService(projectRepository, _mockProjectImageService.Object, _mockProjectRepositoryService.Object, _mockProjectStatService.Object, _mockTagService.Object, NullLogger<ProjectService>.Instance);

        _mockProjectImageService.Setup(service => service.SyncProjectImagesAsync(It.IsAny<Project>(), It.IsAny<IReadOnlyList<CreateProjectImage>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => Result<IReadOnlyList<ProjectImage>>.Ok([]));

        _mockProjectRepositoryService.Setup(service => service.SyncProjectRepositoriesAsync(It.IsAny<Project>(), It.IsAny<IReadOnlyList<CreateProjectRepository>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => Result<IReadOnlyList<ProjectRepositoryEntity>>.Ok([]));

        _mockProjectStatService.Setup(service => service.SyncProjectStatsAsync(It.IsAny<Project>(), It.IsAny<IReadOnlyList<CreateProjectStat>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => Result<IReadOnlyList<ProjectStat>>.Ok([]));
    }

    [Fact]
    public async Task GetProjectsAsync_NoSearchTerm_ReturnsAllProjects()
    {

        var result = (await _projectService.GetProjectsAsync(null)).ExpectSuccess();

        Assert.NotNull(result);
        Assert.Equal(3, result.Count());
    }

    [Fact]
    public async Task GetProjectsAsync_WithSearchTerm_ReturnsMatchingProjects()
    {

        var result = (await _projectService.GetProjectsAsync("ViSIT")).ExpectSuccess().ToList();

        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("Visit Northumberland", result.First().Title);
    }

    [Fact]
    public async Task GetProjectsAsync_OrderedByDateDescending()
    {

        var result = (await _projectService.GetProjectsAsync(null)).ExpectSuccess().ToList();

        Assert.NotNull(result);
        Assert.Equal("BeatCovidNE", result[0].Title);
        Assert.Equal("Visit Northumberland", result[1].Title);
        Assert.Equal("taxigoat", result[2].Title);
    }
    
    [Fact]
    public async Task GetProjectAsync_ValidId_ReturnsProject()
    {
        var testProjectId = Project1Id; // Assuming this ID exists in GetTestProjects()

        var result = (await _projectService.GetProjectAsync(testProjectId)).ExpectSuccess();

        Assert.NotNull(result);
        Assert.Equal(testProjectId, result.Id);
    }

    [Fact]
    public async Task GetProjectBySlugAsync_ValidSlug_ReturnsProject()
    {
        const string validSlug = "client-portal";
        await SaveProject(CreateProject(new ProjectSeed(ProjectId.New(), "Client Portal") { CompanyId = Company1Id }));

        var result = (await _projectService.GetProjectBySlugAsync(validSlug)).ExpectSuccess();

        Assert.NotNull(result);
        Assert.Equal(validSlug, result.Slug);
    }

    [Fact]
    public async Task GetProjectBySlugAsync_InvalidSlug_ThrowsNotFoundFailure()
    {
        const string invalidSlug = "non-existent-slug";

        var result = (await _projectService.GetProjectBySlugAsync(invalidSlug)).ExpectFailure();
        Assert.Equal(ErrorCodes.ProjectNotFound, result);
    }

    [Fact]
    public async Task GetRelatedProjectsAsync_WithCommonTags_ReturnsRelatedProjects()
    {
        var projectIdWithTags = Project1Id;

        var result = (await _projectService.GetRelatedProjectsAsync(projectIdWithTags)).ExpectSuccess();

        Assert.NotNull(result);
        Assert.True(result.Any());
    }

    [Fact]
    public async Task GetRelatedProjectsAsync_GetsProjectsWithMostRelatedTags()
    {
        await ClearDatabase();

        var tag1 = CreateTag(TagId.New(), "Tag 1", TagType.Default);
        var tag2 = CreateTag(TagId.New(), "Tag 2", TagType.Backend);
        var tag3 = CreateTag(TagId.New(), "Tag 3", TagType.Frontend);
        var tag4 = CreateTag(TagId.New(), "Tag 4", TagType.DevOps);
        var tag5 = CreateTag(TagId.New(), "Tag 5", TagType.Data);
        var tag6 = CreateTag(TagId.New(), "Tag 6", TagType.Mobile);

        var mainProject = await SaveProject(CreateProject(ProjectId.New(), "Project 1", tags: [tag1, tag2, tag3, tag4, tag5, tag6]));

        var relatedProject1 = await SaveProject(CreateProject(ProjectId.New(), "Project 2", tags: [tag1, tag2]));
        var relatedProject2 = await SaveProject(CreateProject(ProjectId.New(), "Project 3", tags: [tag1, tag2, tag3]));
        var relatedProject3 = await SaveProject(CreateProject(ProjectId.New(), "Project 4", tags: [tag1, tag2, tag3, tag4]));
        var relatedProject4 = await SaveProject(CreateProject(ProjectId.New(), "Project 5", tags: [tag1, tag2, tag3, tag4, tag5]));
        var relatedProject5 = await SaveProject(CreateProject(ProjectId.New(), "Project 6", tags: [tag1, tag2, tag3, tag4, tag5, tag6]));
        var relatedProjects = (await _projectService.GetRelatedProjectsAsync(mainProject.Id)).ExpectSuccess()!.ToList();

        Assert.NotNull(relatedProjects);
        Assert.Equal(3, relatedProjects.Count);
        Assert.DoesNotContain(relatedProjects, p => p.Id == relatedProject1.Id);
        Assert.DoesNotContain(relatedProjects, p => p.Id == relatedProject2.Id);
        Assert.Contains(relatedProjects, p => p.Id == relatedProject3.Id);
        Assert.Contains(relatedProjects, p => p.Id == relatedProject4.Id);
        Assert.Contains(relatedProjects, p => p.Id == relatedProject5.Id);
    }

    [Fact]
    public async Task CreateProjectAsync_NewProject_ReturnsProject()
    {
        var newProject = new CreateProject
        {
            Title = "Test Project",
            Description = "Test Description",
            ShortDescription = "Test Short Description",
            CompanyId = Company1Id,
            StartDate = new DateOnly(2021, 1, 1),
            EndDate = new DateOnly(2022, 1, 1),
            Website = "https://example.com",
            Tags = ["Test Tag"]
        };

        _mockTagService.Setup(ts => ts.SyncTagsAsync(It.IsAny<IReadOnlyList<string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((IReadOnlyList<string> tags, CancellationToken _) =>
            {
                var createdTags = tags.Select(t => CreateTag(TagId.New(), t, TagType.Default)).ToList();
                Context.Tag.AddRange(createdTags);
                Context.SaveChanges();
                return Result<IReadOnlyList<Tag>>.Ok(createdTags);
            });

        var result = (await _projectService.CreateProjectAsync(newProject)).ExpectSuccess();

        Assert.NotNull(result);
        Assert.Equal(newProject.Title, result.Title);
        Assert.Equal(newProject.Description, result.Description);
        Assert.Equal(newProject.ShortDescription, result.ShortDescription);
        Assert.Equal(newProject.CompanyId, result.CompanyId);
        Assert.Equal(newProject.StartDate, result.StartDate);
        Assert.Equal(newProject.EndDate, result.EndDate);
        Assert.Equal(newProject.Website, result.Website);

        var projectInDb = await Context.Project.FindAsync(result.Id);
        Assert.NotNull(projectInDb);
        Assert.Equal(newProject.Title, projectInDb.Title);
        Assert.Equal(newProject.StartDate, projectInDb.StartDate);
        Assert.Equal(newProject.EndDate, projectInDb.EndDate);
        Assert.NotNull(projectInDb.Tags);
        Assert.Equal(newProject.Tags.Count, projectInDb.Tags.Count);
        Assert.Contains(projectInDb.Tags, t => t.Title == "Test Tag");
    }

    [Fact]
    public async Task CreateProjectAsync_WithCaseStudyFields_ReturnsProjectWithCaseStudyFields()
    {
        var newProject = new CreateProject
        {
            Title = "Case Study Project",
            Description = "Test Description",
            ShortDescription = "Test Short Description",
            ProblemStatement = "Booking took a week over email.",
            KeyDecision = "Live availability over WebSocket, not a bigger form.",
            Outcome = "Confirmation time dropped from days to minutes.",
            StartDate = new DateOnly(2021, 1, 1),
            Tags = []
        };

        _mockTagService.Setup(ts => ts.SyncTagsAsync(It.IsAny<IReadOnlyList<string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => Result<IReadOnlyList<Tag>>.Ok([]));

        var result = (await _projectService.CreateProjectAsync(newProject)).ExpectSuccess();

        Assert.NotNull(result);
        Assert.Equal(newProject.ProblemStatement, result.ProblemStatement);
        Assert.Equal(newProject.KeyDecision, result.KeyDecision);
        Assert.Equal(newProject.Outcome, result.Outcome);

        var projectInDb = await Context.Project.FindAsync(result.Id);
        Assert.NotNull(projectInDb);
        Assert.Equal(newProject.ProblemStatement, projectInDb.ProblemStatement);
        Assert.Equal(newProject.KeyDecision, projectInDb.KeyDecision);
        Assert.Equal(newProject.Outcome, projectInDb.Outcome);
    }

    [Fact]
    public async Task CreateProjectAsync_WithoutCaseStudyFields_LeavesThemNull()
    {
        var newProject = new CreateProject
        {
            Title = "No Case Study Project",
            Description = "Test Description",
            ShortDescription = "Test Short Description",
            StartDate = new DateOnly(2021, 1, 1),
            Tags = []
        };

        _mockTagService.Setup(ts => ts.SyncTagsAsync(It.IsAny<IReadOnlyList<string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => Result<IReadOnlyList<Tag>>.Ok([]));

        var result = (await _projectService.CreateProjectAsync(newProject)).ExpectSuccess();

        Assert.NotNull(result);
        Assert.Null(result.ProblemStatement);
        Assert.Null(result.KeyDecision);
        Assert.Null(result.Outcome);
    }

    [Fact]
    public async Task UpdateProjectAsync_ExistingProject_UpdatesProject()
    {
        var tag = CreateTag(TagId.New(), "Updated Tag", TagType.Backend);
        var existingProject = await SaveProject(CreateProject(new ProjectSeed(ProjectId.New(), "Test Update Project")
        {
            CompanyId = Company1Id,
            StartDate = new DateOnly(2021, 1, 1),
            Tags = [tag]
        }));

        var updateData = new CreateProject
        {
            Title = "Updated Project",
            Description = "Updated Description",
            ShortDescription = "Updated Short Description",
            CompanyId = Company1Id,
            StartDate = new DateOnly(2021, 1, 1),
            EndDate = new DateOnly(2022, 1, 1),
            Website = "https://updated.com",
            Tags = ["Updated Tag"]
        };

        _mockTagService.Setup(ts => ts.SyncTagsAsync(It.IsAny<IReadOnlyList<string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => Result<IReadOnlyList<Tag>>.Ok([tag]));

        var result = (await _projectService.UpdateProjectAsync(existingProject.Id, updateData)).ExpectSuccess();

        Assert.NotNull(result);
        Assert.Equal(updateData.Title, result.Title);
        Assert.Equal(updateData.Description, result.Description);
        Assert.Equal(updateData.ShortDescription, result.ShortDescription);
        Assert.Equal(updateData.CompanyId, result.CompanyId);
        Assert.Equal(updateData.StartDate, result.StartDate);
        Assert.Equal(updateData.EndDate, result.EndDate);
        Assert.Equal(updateData.Website, result.Website);

        var projectInDb = await Context.Project.FindAsync(existingProject.Id);
        Assert.NotNull(projectInDb);
        Assert.Equal(updateData.Title, projectInDb.Title);
        Assert.Equal(updateData.Description, projectInDb.Description);
        Assert.Equal(updateData.ShortDescription, projectInDb.ShortDescription);
        Assert.Equal(updateData.CompanyId, projectInDb.CompanyId);
        Assert.Equal(updateData.StartDate, projectInDb.StartDate);
        Assert.Equal(updateData.EndDate, projectInDb.EndDate);
        Assert.Equal(updateData.Website, projectInDb.Website);
        Assert.NotNull(projectInDb.Tags);
        Assert.Equal(updateData.Tags.Count, projectInDb.Tags.Count);
        Assert.Contains(projectInDb.Tags, t => t.Title == "Updated Tag");
    }

    [Fact]
    public async Task DeleteProjectAsync_ExistingProject_DeletesProject()
    {
        var existingProject = await SaveProject(CreateProject(ProjectId.New(), "Test Delete Project"));

        var result = (await _projectService.DeleteProjectAsync(existingProject.Id)).ExpectSuccess();

        Assert.NotNull(result);
        Assert.Equal(existingProject.Id, result.Id);

        var projectInDb = await Context.Project.FindAsync(existingProject.Id);
        Assert.Null(projectInDb);
    }

    [Fact]
    public async Task GetProjectAsync_InvalidId_ReturnsProjectNotFound()
    {
        var error = (await _projectService.GetProjectAsync(ProjectId.New())).ExpectFailure();

        Assert.Equal(ErrorCodes.ProjectNotFound, error);
    }

    [Fact]
    public async Task CreateProjectAsync_DuplicateTitle_ReturnsProjectAlreadyExists()
    {
        var duplicate = new CreateProject
        {
            Title = "visit northumberland",
            Description = "Description",
            ShortDescription = "Short Description",
            StartDate = new DateOnly(2021, 1, 1)
        };

        var error = (await _projectService.CreateProjectAsync(duplicate)).ExpectFailure();

        Assert.Equal(ErrorCodes.ProjectAlreadyExists, error);
    }

    [Fact]
    public async Task CreateProjectAsync_RelationSyncFails_PropagatesErrorAndRemovesProject()
    {
        _mockTagService.Setup(ts => ts.SyncTagsAsync(It.IsAny<IReadOnlyList<string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => Result<IReadOnlyList<Tag>>.Ok([]));
        _mockProjectImageService.Setup(service => service.SyncProjectImagesAsync(It.IsAny<Project>(), It.IsAny<IReadOnlyList<CreateProjectImage>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => Result<IReadOnlyList<ProjectImage>>.Fail(ErrorCodes.FileRequired));
        var request = new CreateProject
        {
            Title = "Project With Broken Image",
            Description = "Description",
            ShortDescription = "Short Description",
            StartDate = new DateOnly(2021, 1, 1)
        };

        var error = (await _projectService.CreateProjectAsync(request)).ExpectFailure();

        Assert.Equal(ErrorCodes.FileRequired, error);
        Assert.DoesNotContain(Context.Project, p => p.Title == request.Title);
    }

    [Fact]
    public async Task UpdateProjectAsync_InvalidId_ReturnsProjectNotFound()
    {
        var update = new CreateProject
        {
            Title = "Missing",
            Description = "Description",
            ShortDescription = "Short Description",
            StartDate = new DateOnly(2021, 1, 1)
        };

        var error = (await _projectService.UpdateProjectAsync(ProjectId.New(), update)).ExpectFailure();

        Assert.Equal(ErrorCodes.ProjectNotFound, error);
    }

    [Fact]
    public async Task UpdateProjectAsync_TitleTakenByAnotherProject_ReturnsProjectAlreadyExists()
    {
        var update = new CreateProject
        {
            Title = "BeatCovidNE",
            Description = "Description",
            ShortDescription = "Short Description",
            StartDate = new DateOnly(2021, 1, 1)
        };

        var error = (await _projectService.UpdateProjectAsync(Project1Id, update)).ExpectFailure();

        Assert.Equal(ErrorCodes.ProjectAlreadyExists, error);
    }

    [Fact]
    public async Task DeleteProjectAsync_InvalidId_ReturnsProjectNotFound()
    {
        var error = (await _projectService.DeleteProjectAsync(ProjectId.New())).ExpectFailure();

        Assert.Equal(ErrorCodes.ProjectNotFound, error);
    }
}
