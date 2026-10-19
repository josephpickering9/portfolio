using System.Net;
using System.Text;
using Work_Experience_Search.Requests;
using Work_Experience_Search.Models;
using WorkExperienceSearchTests.Tests.Fixtures;
using Work_Experience_Search.Types;
using Xunit;

namespace WorkExperienceSearchTests.Tests.Integration;

public class TagControllerIntegrationTests(CustomWebApplicationFactory customWebApplicationFactory)
    : BaseControllerIntegrationTests(customWebApplicationFactory), IClassFixture<CustomWebApplicationFactory>
{
    [Fact]
    public async Task GetTags_ReturnsTags()
    {
        var tags = new List<Tag>
        {
            await CreateTagAsync(TagId.New(), "Tag 1"),
            await CreateTagAsync(TagId.New(), "Tag 2"),
            await CreateTagAsync(TagId.New(), "Tag 3")
        };

        var httpResponse = await Client.GetAsync("/tag");
        httpResponse.EnsureSuccessStatusCode();
        var stringResponse = await httpResponse.Content.ReadAsStringAsync();
        var response = GetJsonContent<List<Tag>>(stringResponse);

        Assert.NotNull(response);
        Assert.NotEmpty(response);
        Assert.Equal(tags.Count, response.Count);
    }

    [Fact]
    public async Task GetTag_ExistingId_ReturnsTag()
    {
        var testTagId = TagId.New();
        await CreateTagAsync(testTagId);

        var httpResponse = await Client.GetAsync($"/tag/{testTagId}");
        httpResponse.EnsureSuccessStatusCode();
        var stringResponse = await httpResponse.Content.ReadAsStringAsync();
        var tag = GetJsonContent<Tag>(stringResponse);

        Assert.NotNull(tag);
        Assert.Equal(testTagId, tag.Id);
    }

    [Fact]
    public async Task GetTag_NonExistingId_ReturnsNotFound()
    {
        var nonExistingTagId = TagId.New();

        var httpResponse = await Client.GetAsync($"/tag/{nonExistingTagId}");

        await AssertErrorAsync(httpResponse, HttpStatusCode.NotFound, ErrorCodes.TagNotFound);
    }

    [Fact]
    public async Task PostTag_Unauthorized_ReturnsUnauthorized()
    {
        var createTag = new CreateTag
        {
            Title = "New Tag",
            Type = TagType.Frontend,
            Icon = "",
            CustomColour = null
        };

        var httpResponse = await Client.PostAsync("/tag", CreateJsonContent(createTag));

        await AssertErrorAsync(httpResponse, HttpStatusCode.Unauthorized, ErrorCodes.Unauthorized);
    }

    [Fact]
    public async Task PostTag_ValidTag_ReturnsTag()
    {
        var createTag = new CreateTag
        {
            Title = "New Tag",
            Type = TagType.Frontend,
            Icon = "",
            CustomColour = null
        };

        var httpResponse = await AuthenticatedClient.PostAsync("/tag", CreateJsonContent(createTag));
        Assert.Equal(HttpStatusCode.Created, httpResponse.StatusCode);
        var stringResponse = await httpResponse.Content.ReadAsStringAsync();
        var tag = GetJsonContent<Tag>(stringResponse);

        Assert.NotNull(tag);
        Assert.EndsWith($"/tag/{tag.Id}", httpResponse.Headers.Location?.ToString());
        Assert.Equal(createTag.Title, tag.Title);
        Assert.Equal(createTag.Type, tag.Type);
        Assert.Equal(createTag.Icon, tag.Icon);
        Assert.Equal(createTag.CustomColour, tag.CustomColour);
    }

    [Fact]
    public async Task PostTag_ExistingTag_ReturnsConflict()
    {
        await CreateTagAsync(TagId.New(), "Conflict Tag");
        var createTag = new CreateTag
        {
            Title = "Conflict Tag",
            Type = TagType.Backend,
            Icon = "",
            CustomColour = null
        };

        var httpResponse = await AuthenticatedClient.PostAsync("/tag", CreateJsonContent(createTag));

        await AssertErrorAsync(httpResponse, HttpStatusCode.Conflict, ErrorCodes.TagAlreadyExists);
    }

    [Fact]
    public async Task PutTag_Unauthorized_ReturnsUnauthorized()
    {
        var updateTag = new CreateTag
        {
            Title = "Updated Tag",
            Type = TagType.Backend,
            Icon = "",
            CustomColour = null
        };

        var httpResponse = await Client.PutAsync($"/tag/{TagId.New()}", CreateJsonContent(updateTag));

        await AssertErrorAsync(httpResponse, HttpStatusCode.Unauthorized, ErrorCodes.Unauthorized);
    }

    [Fact]
    public async Task PutTag_ValidId_ReturnsUpdatedTag()
    {
        var tagId = TagId.New();
        await CreateTagAsync(tagId);
        var updateTag = new CreateTag
        {
            Title = "Updated Tag",
            Type = TagType.Backend,
            Icon = "",
            CustomColour = null
        };

        var httpResponse = await AuthenticatedClient.PutAsync($"/tag/{tagId}", CreateJsonContent(updateTag));
        httpResponse.EnsureSuccessStatusCode();
        var stringResponse = await httpResponse.Content.ReadAsStringAsync();
        var tag = GetJsonContent<Tag>(stringResponse);

        Assert.NotNull(tag);
        Assert.Equal(tagId, tag.Id);
        Assert.Equal(updateTag.Title, tag.Title);
    }

    [Fact]
    public async Task PutTag_NonExistingId_ReturnsNotFound()
    {
        var nonExistingTagId = TagId.New();
        var updateTag = new CreateTag
        {
            Title = "Updated Tag",
            Type = TagType.Backend,
            Icon = "",
            CustomColour = null
        };

        var httpResponse = await AuthenticatedClient.PutAsync($"/tag/{nonExistingTagId}", CreateJsonContent(updateTag));

        await AssertErrorAsync(httpResponse, HttpStatusCode.NotFound, ErrorCodes.TagNotFound);
    }

    [Fact]
    public async Task DeleteTag_Unauthorized_ReturnsUnauthorized()
    {
        var httpResponse = await Client.DeleteAsync($"/tag/{TagId.New()}");

        await AssertErrorAsync(httpResponse, HttpStatusCode.Unauthorized, ErrorCodes.Unauthorized);
    }

    [Fact]
    public async Task DeleteTag_ValidId_DeletesTag()
    {
        var tagId = TagId.New();
        await CreateTagAsync(tagId);

        var httpResponse = await AuthenticatedClient.DeleteAsync($"/tag/{tagId}");
        Assert.Equal(HttpStatusCode.NoContent, httpResponse.StatusCode);
    }

    [Fact]
    public async Task DeleteTag_NonExistingId_ReturnsNotFound()
    {
        var nonExistingTagId = TagId.New();

        var httpResponse = await AuthenticatedClient.DeleteAsync($"/tag/{nonExistingTagId}");

        await AssertErrorAsync(httpResponse, HttpStatusCode.NotFound, ErrorCodes.TagNotFound);
    }

    [Fact]
    public async Task GetTags_NoTags_ReturnsEmptyList()
    {
        var httpResponse = await Client.GetAsync("/tag");

        httpResponse.EnsureSuccessStatusCode();
        var response = GetJsonContent<List<Tag>>(await httpResponse.Content.ReadAsStringAsync());
        Assert.NotNull(response);
        Assert.Empty(response);
    }

    [Fact]
    public async Task GetTag_ExistingSlug_ReturnsTag()
    {
        var tag = await CreateTagAsync(TagId.New());

        var httpResponse = await Client.GetAsync($"/tag/{tag.Slug}");

        httpResponse.EnsureSuccessStatusCode();
        var response = GetJsonContent<Tag>(await httpResponse.Content.ReadAsStringAsync());
        Assert.NotNull(response);
        Assert.Equal(tag.Id, response.Id);
    }

    [Fact]
    public async Task GetTag_NonExistingSlug_ReturnsNotFound()
    {
        var httpResponse = await Client.GetAsync("/tag/non-existent-tag");

        await AssertErrorAsync(httpResponse, HttpStatusCode.NotFound, ErrorCodes.TagNotFound);
    }

    [Fact]
    public async Task PostTag_InvalidCustomColour_ReturnsValidationCode()
    {
        var createTag = new CreateTag { Title = "Colourful", Type = TagType.Frontend, CustomColour = "red" };

        var httpResponse = await AuthenticatedClient.PostAsync("/tag", CreateJsonContent(createTag));

        await AssertErrorAsync(httpResponse, HttpStatusCode.BadRequest, ErrorCodes.CustomColourInvalid);
    }

    [Fact]
    public async Task PostTag_EmptyTitle_ReturnsValidationCode()
    {
        var createTag = new CreateTag { Title = "", Type = TagType.Frontend };

        var httpResponse = await AuthenticatedClient.PostAsync("/tag", CreateJsonContent(createTag));

        await AssertErrorAsync(httpResponse, HttpStatusCode.BadRequest, ErrorCodes.TitleRequired);
    }

    [Fact]
    public async Task PostTag_MalformedBody_ReturnsBadRequestCode()
    {
        var content = new StringContent("{ not json", Encoding.UTF8, "application/json");

        var httpResponse = await AuthenticatedClient.PostAsync("/tag", content);

        await AssertErrorAsync(httpResponse, HttpStatusCode.BadRequest, ErrorCodes.BadRequest);
    }

    [Fact]
    public async Task PostTag_UnsupportedContentType_ReturnsBadRequestCode()
    {
        var content = new StringContent("Title=Plain", Encoding.UTF8, "text/plain");

        var httpResponse = await AuthenticatedClient.PostAsync("/tag", content);

        await AssertErrorAsync(httpResponse, HttpStatusCode.BadRequest, ErrorCodes.BadRequest);
    }
}
