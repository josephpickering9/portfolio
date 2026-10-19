using System.Net;
using Work_Experience_Search.Models;
using Work_Experience_Search.Requests;
using WorkExperienceSearchTests.Tests.Fixtures;
using Work_Experience_Search.Types;
using Xunit;

namespace WorkExperienceSearchTests.Tests.Integration;

public class ProjectControllerFailureIntegrationTests(CustomWebApplicationFactory customWebApplicationFactory)
    : BaseControllerIntegrationTests(customWebApplicationFactory), IClassFixture<CustomWebApplicationFactory>
{
    [Fact]
    public async Task GetProjects_NoProjects_ReturnsEmptyList()
    {
        var httpResponse = await Client.GetAsync("/project");

        httpResponse.EnsureSuccessStatusCode();
        var response = GetJsonContent<List<Project>>(await httpResponse.Content.ReadAsStringAsync());
        Assert.NotNull(response);
        Assert.Empty(response);
    }

    [Fact]
    public async Task GetRelatedProjects_ProjectWithoutTags_ReturnsEmptyList()
    {
        var project = await CreateProjectAsync(ProjectId.New());

        var httpResponse = await Client.GetAsync($"/project/{project.Id}/related");

        httpResponse.EnsureSuccessStatusCode();
        var response = GetJsonContent<List<Project>>(await httpResponse.Content.ReadAsStringAsync());
        Assert.NotNull(response);
        Assert.Empty(response);
    }

    [Fact]
    public async Task PutProject_NonExistingId_ReturnsNotFound()
    {
        var content = GetMultipartFormDataContent(ValidProject("Missing Project"));

        var httpResponse = await AuthenticatedClient.PutAsync($"/project/{ProjectId.New()}", content);

        await AssertErrorAsync(httpResponse, HttpStatusCode.NotFound, ErrorCodes.ProjectNotFound);
    }

    [Fact]
    public async Task DeleteProject_NonExistingId_ReturnsNotFound()
    {
        var httpResponse = await AuthenticatedClient.DeleteAsync($"/project/{ProjectId.New()}");

        await AssertErrorAsync(httpResponse, HttpStatusCode.NotFound, ErrorCodes.ProjectNotFound);
    }

    [Fact]
    public async Task PostProject_EmptyTitle_ReturnsValidationCode()
    {
        var content = GetMultipartFormDataContent(ValidProject(string.Empty));

        var httpResponse = await AuthenticatedClient.PostAsync("/project", content);

        await AssertErrorAsync(httpResponse, HttpStatusCode.BadRequest, ErrorCodes.TitleRequired);
    }

    [Fact]
    public async Task PostProject_EndDateBeforeStartDate_ReturnsValidationCode()
    {
        var content = GetMultipartFormDataContent(ValidProject("Backwards Project") with
        {
            StartDate = new DateOnly(2022, 1, 1),
            EndDate = new DateOnly(2021, 1, 1)
        });

        var httpResponse = await AuthenticatedClient.PostAsync("/project", content);

        await AssertErrorAsync(httpResponse, HttpStatusCode.BadRequest, ErrorCodes.EndDateBeforeStartDate);
    }

    [Fact]
    public async Task PostProject_NewImageWithoutFile_ReturnsFileRequired()
    {
        var content = new MultipartFormDataContent
        {
            { new StringContent("Project Missing Image"), "Title" },
            { new StringContent("A short description"), "ShortDescription" },
            { new StringContent("A long description"), "Description" },
            { new StringContent("2021-01-01"), "StartDate" },
            { new StringContent(nameof(ImageType.Desktop)), "Images[0].Type" },
        };

        var httpResponse = await AuthenticatedClient.PostAsync("/project", content);

        await AssertErrorAsync(httpResponse, HttpStatusCode.BadRequest, ErrorCodes.FileRequired);
    }

    [Fact]
    public async Task OptimiseImages_Unauthorized_ReturnsUnauthorized()
    {
        var httpResponse = await Client.PutAsync("/projectimage/optimise", null);

        await AssertErrorAsync(httpResponse, HttpStatusCode.Unauthorized, ErrorCodes.Unauthorized);
    }

    [Fact]
    public async Task OptimiseImages_Authorized_ReturnsNoContent()
    {
        var httpResponse = await AuthenticatedClient.PutAsync("/projectimage/optimise", null);

        Assert.Equal(HttpStatusCode.NoContent, httpResponse.StatusCode);
    }

    private static CreateProject ValidProject(string title) =>
        new()
        {
            Title = title,
            ShortDescription = "A short description",
            Description = "A long description",
            StartDate = new DateOnly(2021, 1, 1),
            EndDate = new DateOnly(2022, 1, 1),
            Website = "https://example.com"
        };
}
