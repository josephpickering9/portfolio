using System.Net;
using Portfolio.Api.Models;
using Portfolio.Api.Requests;
using Portfolio.Api.Types;
using Portfolio.Api.Tests.Fixtures;
using Xunit;

namespace Portfolio.Api.Tests.Integration;

public class SlugAndSearchIntegrationTests(CustomWebApplicationFactory customWebApplicationFactory)
    : BaseControllerIntegrationTests(customWebApplicationFactory), IClassFixture<CustomWebApplicationFactory>
{
    private const int MaxSearchLength = 100;

    [Fact]
    public async Task PostTag_TitlesWithSameSlug_BothCreatedWithDistinctSlugs()
    {
        var first = await PostTagAsync("C#");
        var second = await PostTagAsync("C");

        Assert.Equal("c", first.Slug);
        Assert.Equal("c-2", second.Slug);

        var bySlug = await Client.GetAsync("/tag/c-2");
        bySlug.EnsureSuccessStatusCode();
        Assert.Equal(second.Id, GetJsonContent<Tag>(await bySlug.Content.ReadAsStringAsync())?.Id);
    }

    [Fact]
    public async Task PutTag_TitleUnchanged_KeepsSlug()
    {
        await PostTagAsync("C#");
        var tag = await PostTagAsync("C");

        var httpResponse = await AuthenticatedClient.PutAsync($"/tag/{tag.Id}", CreateJsonContent(NewTag("C", TagType.Backend)));

        httpResponse.EnsureSuccessStatusCode();
        Assert.Equal("c-2", GetJsonContent<Tag>(await httpResponse.Content.ReadAsStringAsync())?.Slug);
    }

    [Fact]
    public async Task PostTag_TitleWithoutSlugCharacters_UsesFallbackSlug()
    {
        var tag = await PostTagAsync("!!!");

        Assert.Equal("tag", tag.Slug);
    }

    [Fact]
    public async Task PostTag_TitleIsLikeWildcard_IsNotTreatedAsDuplicate()
    {
        await PostTagAsync("Rust");

        var httpResponse = await AuthenticatedClient.PostAsync("/tag", CreateJsonContent(NewTag("%")));

        Assert.Equal(HttpStatusCode.Created, httpResponse.StatusCode);
    }

    [Fact]
    public async Task PostTag_SameTitleDifferentCase_ReturnsConflict()
    {
        await PostTagAsync("Rust");

        var httpResponse = await AuthenticatedClient.PostAsync("/tag", CreateJsonContent(NewTag("rUST")));

        await AssertErrorAsync(httpResponse, HttpStatusCode.Conflict, ErrorCodes.TagAlreadyExists);
    }

    [Fact]
    public async Task GetTags_SearchContainsWildcard_MatchesLiterally()
    {
        await CreateTagAsync(TagId.New(), "Rust");
        await CreateTagAsync(TagId.New(), "snake_case");

        var underscore = await GetTagTitlesAsync("_");
        var percent = await GetTagTitlesAsync("%");

        Assert.Equal(["snake_case"], underscore);
        Assert.Empty(percent);
    }

    [Fact]
    public async Task GetTags_SearchIsCaseInsensitive()
    {
        await CreateTagAsync(TagId.New(), "TypeScript");

        Assert.Equal(["TypeScript"], await GetTagTitlesAsync("script"));
    }

    [Theory]
    [InlineData("/project")]
    [InlineData("/company")]
    [InlineData("/tag")]
    public async Task GetList_SearchTooLong_ReturnsValidationCode(string path)
    {
        var httpResponse = await Client.GetAsync($"{path}?search={new string('a', MaxSearchLength + 1)}");

        await AssertErrorAsync(httpResponse, HttpStatusCode.BadRequest, ErrorCodes.SearchTooLong);
    }

    private async Task<Tag> PostTagAsync(string title)
    {
        var httpResponse = await AuthenticatedClient.PostAsync("/tag", CreateJsonContent(NewTag(title)));
        Assert.Equal(HttpStatusCode.Created, httpResponse.StatusCode);
        return GetJsonContent<Tag>(await httpResponse.Content.ReadAsStringAsync())
            ?? throw new InvalidOperationException("Created tag body was empty.");
    }

    private async Task<IReadOnlyList<string>> GetTagTitlesAsync(string search)
    {
        var httpResponse = await Client.GetAsync($"/tag?search={Uri.EscapeDataString(search)}");
        httpResponse.EnsureSuccessStatusCode();
        var tags = GetJsonContent<List<Tag>>(await httpResponse.Content.ReadAsStringAsync()) ?? [];
        return [.. tags.Select(tag => tag.Title)];
    }

    private static CreateTag NewTag(string title, TagType type = TagType.Frontend) =>
        new() { Title = title, Type = type };
}
