using Microsoft.EntityFrameworkCore;
using Work_Experience_Search.Models;
using Work_Experience_Search.Services;
using Work_Experience_Search.Types;
using Work_Experience_Search.Extensions;
using WorkExperienceSearchTests.Tests.Fixtures;
using Xunit;

namespace WorkExperienceSearchTests.Tests.Unit.Services;

public abstract class BaseServiceTests : IAsyncLifetime
{
    protected readonly Database Context;
    protected static readonly CompanyId Company1Id = new(Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"));
    protected static readonly ProjectId Project1Id = new(Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"));
    protected static readonly ProjectId Project2Id = new(Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"));
    protected static readonly ProjectId Project3Id = new(Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd"));
    protected static readonly TagId Tag1Id = new(Guid.Parse("11111111-1111-1111-1111-111111111111"));
    protected static readonly TagId Tag2Id = new(Guid.Parse("22222222-2222-2222-2222-222222222222"));
    protected static readonly TagId Tag3Id = new(Guid.Parse("33333333-3333-3333-3333-333333333333"));
    protected static readonly ProjectImageId Image1Id = new(Guid.Parse("44444444-4444-4444-4444-444444444441"));
    protected static readonly ProjectImageId Image2Id = new(Guid.Parse("44444444-4444-4444-4444-444444444442"));
    protected static readonly ProjectImageId Image3Id = new(Guid.Parse("44444444-4444-4444-4444-444444444443"));
    protected static readonly ProjectImageId Image4Id = new(Guid.Parse("44444444-4444-4444-4444-444444444444"));
    protected static readonly ProjectImageId Image5Id = new(Guid.Parse("44444444-4444-4444-4444-444444444445"));
    protected static readonly ProjectImageId Image6Id = new(Guid.Parse("44444444-4444-4444-4444-444444444446"));
    protected static readonly ProjectImageId Image7Id = new(Guid.Parse("44444444-4444-4444-4444-444444444447"));

    private string? _connectionString;

    protected BaseServiceTests()
    {
        Context = new Database(new DbContextOptionsBuilder<Database>().UseNpgsql().Options);
    }

    public async Task InitializeAsync()
    {
        _connectionString = await TestDatabase.CreateAsync();
        Context.Database.SetConnectionString(_connectionString);
        await SeedDatabase();
    }

    public async Task DisposeAsync()
    {
        await Context.DisposeAsync();
        if (_connectionString is not null) await TestDatabase.DropAsync(_connectionString);
    }
    
    private async Task SeedDatabase()
    {
        Context.Tag.AddRange(await GetTestTags());
        Context.Company.AddRange(GetTestCompanies());
        Context.Project.AddRange(await GetTestProjects());
        Context.ProjectImage.AddRange(GetTestProjectImages());
        await Context.SaveChangesAsync();
    }

    protected Task ClearDatabase() => Context.TruncateAllTablesAsync();

    protected static Tag CreateTag(TagId id, string title, TagType type, List<Project>? projects = null)
    {
        return new Tag
        {
            Id = id,
            Title = title,
            Type = type,
            Icon = "testIcon",
            CustomColour = null,
            Projects = projects ?? []
        };
    }

    protected sealed record ProjectSeed(ProjectId Id, string Title)
    {
        public string Description { get; init; } = "Description";
        public string ShortDescription { get; init; } = "Short Description";
        public CompanyId? CompanyId { get; init; }
        public DateOnly StartDate { get; init; } = new(2020, 1, 1);
        public DateOnly? EndDate { get; init; }
        public string? Website { get; init; }
        public List<Tag> Tags { get; init; } = [];
    }

    protected static Project CreateProject(ProjectId id, string title = "Title", List<Tag>? tags = null) =>
        CreateProject(new ProjectSeed(id, title) { Tags = tags ?? [] });

    protected static Project CreateProject(ProjectSeed seed) =>
        new()
        {
            Id = seed.Id,
            Title = seed.Title,
            Description = seed.Description,
            ShortDescription = seed.ShortDescription,
            CompanyId = seed.CompanyId,
            StartDate = seed.StartDate,
            EndDate = seed.EndDate ?? seed.StartDate.AddYears(1),
            Website = seed.Website,
            Tags = seed.Tags,
            Slug = seed.Title.ToSlug()
        };

    private static ProjectImage CreateProjectImage(
        Guid id,
        string image,
        ImageType type,
        int? order = null,
        Guid? projectId = null!
    ) =>
        CreateProjectImage(new ProjectImageId(id), image, type, order,
            projectId.HasValue ? new ProjectId(projectId.Value) : null);

    private static ProjectImage CreateProjectImage(
        ProjectImageId id,
        string image,
        ImageType type,
        int? order = null,
        ProjectId? projectId = null!
    )
    {
        var resolvedProjectId = projectId ?? Project1Id;
        return new ProjectImage
        {
            Id = id,
            Image = image,
            Type = type,
            Order = order,
            ProjectId = resolvedProjectId
        };
    }

    private async Task<IEnumerable<Tag>> GetTestTags()
    {
        var cSharpTag = CreateTag(Tag1Id, "C#", TagType.Backend);
        var aspNetCoreTag = CreateTag(Tag2Id, "ASP.NET Core", TagType.Backend);
        var xamarinFormsTag = CreateTag(Tag3Id, "Xamarin Forms", TagType.Frontend);

        List<Tag> tags = [cSharpTag, aspNetCoreTag, xamarinFormsTag];
        var returnTags = new List<Tag>();
        foreach (var tag in tags)
        {
            var existing = await Context.Tag.FindAsync(tag.Id);
            returnTags.Add(existing ?? tag);
        }

        return returnTags;
    }
    
    private static IEnumerable<Company> GetTestCompanies() =>
        [
            new Company
            {
                Id = Company1Id,
                Name = "Test Company",
                Description = "Test Description",
                StartDate = new DateOnly(2020, 1, 1),
                EndDate = new DateOnly(2021, 1, 1),
                Logo = "testLogo",
                Website = "https://example.com"
            }
        ];
    
    private async Task<IEnumerable<Project>> GetTestProjects()
    {
        var tags = (await GetTestTags()).ToList();
        var companies = GetTestCompanies().ToList();

        return
        [
            CreateProject(new ProjectSeed(Project1Id, "Visit Northumberland")
            {
                Description = "A website for Visit Northumberland using C# and ASP.NET Core MVC.",
                ShortDescription = "A website for Visit Northumberland",
                CompanyId = companies.First().Id,
                StartDate = new DateOnly(2020, 1, 1),
                EndDate = new DateOnly(2021, 1, 1),
                Website = "https://visitnorthumberland.com/",
                Tags = [tags[0], tags[1]]
            }),
            CreateProject(new ProjectSeed(Project2Id, "BeatCovidNE")
            {
                Description = "A website for BeatCovidNE using C# and ASP.NET Core MVC.",
                ShortDescription = "A website for BeatCovidNE",
                CompanyId = companies.First().Id,
                StartDate = new DateOnly(2021, 1, 1),
                EndDate = new DateOnly(2022, 1, 1),
                Website = "https://beatcovidne.co.uk/",
                Tags = [tags[1], tags[2]]
            }),
            CreateProject(new ProjectSeed(Project3Id, "taxigoat")
            {
                Description = "A website & mobile application for taxigoat using Xamarin Forms and ASP.NET Core API.",
                ShortDescription = "A website for taxigoat",
                CompanyId = companies.First().Id,
                StartDate = new DateOnly(2019, 1, 1),
                EndDate = new DateOnly(2020, 1, 1),
                Website = "https://taxigoat.co.uk/",
                Tags = [tags[2]]
            })
        ];
    }
    
    private IEnumerable<ProjectImage> GetTestProjectImages(ProjectId projectId = default)
    {
        projectId = projectId == default ? Project1Id : projectId;
        var testLogo = CreateProjectImage(Image1Id, "testLogo.png", ImageType.Logo, projectId: projectId);
        var testBanner = CreateProjectImage(Image2Id, "testBanner.png", ImageType.Banner, projectId: projectId);
        var testCard = CreateProjectImage(Image3Id, "testCard.png", ImageType.Card, projectId: projectId);
        var testDesktop1 = CreateProjectImage(Image4Id, "testDesktop1.png", ImageType.Desktop, 1, projectId);
        var testDesktop2 = CreateProjectImage(Image5Id, "testDesktop2.png", ImageType.Desktop, 2, projectId);
        var testMobile1 = CreateProjectImage(Image6Id, "testMobile1.png", ImageType.Mobile, 1, projectId);
        var testMobile2 = CreateProjectImage(Image7Id, "testMobile2.png", ImageType.Mobile, 2, projectId);

        return
        [
            testLogo,
            testBanner,
            testCard,
            testDesktop1,
            testDesktop2,
            testMobile1,
            testMobile2
        ];
    }
    
    protected async Task<Project> SaveProject(Project project)
    {
        await Context.Project.AddAsync(project);
        await Context.SaveChangesAsync();
        return project;
    }
}
