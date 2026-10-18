using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Abstractions;
using Work_Experience_Search.Requests;
using Work_Experience_Search.Models;
using Work_Experience_Search.Repositories;
using Work_Experience_Search.Services;
using Work_Experience_Search.Types;
using Work_Experience_Search.Extensions;
using WorkExperienceSearchTests.Tests;
using Xunit;

namespace WorkExperienceSearchTests.Tests.Unit.Services;

public class TagServiceTests : BaseServiceTests
{
    private readonly TagService _tagService;

    public TagServiceTests()
    {
        _tagService = new TagService(new TagRepository(Context, new MemoryCache(new MemoryCacheOptions()), new CacheInvalidator()), NullLogger<TagService>.Instance);
    }

    [Fact]
    public async Task GetTagsAsync_NoSearch_ReturnsAllTags()
    {
        var result = (await _tagService.GetTagsAsync(null)).ExpectSuccess();

        Assert.NotNull(result);
        Assert.Equal(3, result.Count());
    }

    [Fact]
    public async Task GetTagAsync_ValidId_ReturnsTag()
    {
        var tagId = Tag1Id;

        var result = (await _tagService.GetTagAsync(tagId)).ExpectSuccess();

        Assert.NotNull(result);
        Assert.Equal(tagId, result.Id);
    }

    [Fact]
    public async Task GetTagAsync_InvalidId_ThrowsNotFoundFailure()
    {
        var tagId = TagId.New();

        var result = (await _tagService.GetTagAsync(tagId)).ExpectFailure();
        Assert.Equal(ErrorCodes.TagNotFound, result);
    }

    [Fact]
    public async Task CreateTagAsync_NewTag_ReturnsTag()
    {
        var createTag = new CreateTag
        {
            Title = "Vue",
            Type = TagType.Frontend,
            Icon = "",
            CustomColour = null
        };

        var result = (await _tagService.CreateTagAsync(createTag)).ExpectSuccess();

        Assert.NotNull(result);
        Assert.Equal(createTag.Title, result.Title);
    }

    [Fact]
    public async Task CreateTagAsync_ExistingTag_ThrowsConflictFailure()
    {
        var createTag = new CreateTag
        {
            Title = "C#",
            Type = TagType.Backend,
            Icon = "",
            CustomColour = null
        };

        var result = (await _tagService.CreateTagAsync(createTag)).ExpectFailure();
        Assert.Equal(ErrorCodes.TagAlreadyExists, result);
    }

    [Fact]
    public async Task UpdateTagAsync_ValidId_ReturnsUpdatedTag()
    {
        var tagId = Tag1Id;
        var updateTag = new CreateTag
        {
            Title = "Updated Tag",
            Type = TagType.Backend,
            Icon = "",
            CustomColour = null
        };

        var result = (await _tagService.UpdateTagAsync(tagId, updateTag)).ExpectSuccess();

        Assert.NotNull(result);
        Assert.Equal(tagId, result.Id);
        Assert.Equal(updateTag.Title, result.Title);
    }

    [Fact]
    public async Task UpdateTagAsync_InvalidId_ThrowsNotFoundFailure()
    {
        var tagId = TagId.New();
        var updateTag = new CreateTag
        {
            Title = "Updated Tag",
            Type = TagType.Backend,
            Icon = "",
            CustomColour = null
        };

        var result = (await _tagService.UpdateTagAsync(tagId, updateTag)).ExpectFailure();
        Assert.Equal(ErrorCodes.TagNotFound, result);
    }

    [Fact]
    public async Task DeleteTagAsync_ValidId_DeletesTag()
    {
        var tagId = Tag1Id;

        var result = (await _tagService.DeleteTagAsync(tagId)).ExpectSuccess();

        Assert.NotNull(result);
        Assert.Equal(tagId, result.Id);

        var tagInDb = await Context.Tag.FindAsync(tagId);
        Assert.Null(tagInDb);
    }

    [Fact]
    public async Task DeleteTagAsync_InvalidId_ThrowsNotFoundFailure()
    {
        var tagId = TagId.New();

        var result = (await _tagService.DeleteTagAsync(tagId)).ExpectFailure();
        Assert.Equal(ErrorCodes.TagNotFound, result);
    }

    [Fact]
    public async Task GetTagBySlugAsync_InvalidSlug_ReturnsTagNotFound()
    {
        var error = (await _tagService.GetTagBySlugAsync("non-existent-tag")).ExpectFailure();

        Assert.Equal(ErrorCodes.TagNotFound, error);
    }

    [Fact]
    public async Task UpdateTagAsync_TitleTakenByAnotherTag_ReturnsTagAlreadyExists()
    {
        var update = new CreateTag { Title = "ASP.NET Core", Type = TagType.Backend };

        var error = (await _tagService.UpdateTagAsync(Tag1Id, update)).ExpectFailure();

        Assert.Equal(ErrorCodes.TagAlreadyExists, error);
    }

    [Fact]
    public async Task SyncTagsAsync_MixOfExistingAndNewTitles_ReusesExistingAndCreatesNew()
    {
        var result = (await _tagService.SyncTagsAsync(["c#", "Rust"])).ExpectSuccess();

        Assert.Equal(2, result.Count);
        Assert.Equal(Tag1Id, result[0].Id);
        Assert.Equal("Rust", result[1].Title);
        Assert.Contains(Context.Tag, t => t.Title == "Rust");
    }
}
