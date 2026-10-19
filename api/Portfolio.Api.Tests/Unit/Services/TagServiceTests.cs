using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Abstractions;
using Portfolio.Api.Requests;
using Portfolio.Api.Models;
using Portfolio.Api.Repositories;
using Portfolio.Api.Services;
using Portfolio.Api.Types;
using Portfolio.Api.Extensions;
using Portfolio.Api.Tests;
using Xunit;

namespace Portfolio.Api.Tests.Unit.Services;

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

        (await _tagService.DeleteTagAsync(tagId)).ExpectSuccess();

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

    [Fact]
    public async Task SyncTagsAsync_ExistingTitleContainsLikeWildcard_DoesNotMatchOtherTitles()
    {
        var existing = (await _tagService.SyncTagsAsync(["Go_"])).ExpectSuccess().Single();

        var synced = (await _tagService.SyncTagsAsync(["GoX"])).ExpectSuccess().Single();

        Assert.NotEqual(existing.Id, synced.Id);
        Assert.Equal("GoX", synced.Title);
    }

    [Fact]
    public async Task SyncTagsAsync_ExistingTitleDifferentCase_ReusesTag()
    {
        var synced = (await _tagService.SyncTagsAsync(["c#"])).ExpectSuccess().Single();

        Assert.Equal(Tag1Id, synced.Id);
    }

    [Fact]
    public async Task SyncTagsAsync_NewTitlesSharingSlug_GetDistinctSlugs()
    {
        var synced = (await _tagService.SyncTagsAsync(["Vue.js", "Vuejs"])).ExpectSuccess();

        Assert.Equal(["vuejs", "vuejs-2"], synced.Select(tag => tag.Slug));
    }
}
