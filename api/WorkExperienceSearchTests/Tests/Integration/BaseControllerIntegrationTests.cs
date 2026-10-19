using System.Net;
using System.Net.Http.Headers;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Work_Experience_Search.Models;
using Work_Experience_Search.Services;
using WorkExperienceSearchTests.Tests.Fixtures;
using Work_Experience_Search.Extensions;
using Work_Experience_Search.Types;
using Xunit;

namespace WorkExperienceSearchTests.Tests.Integration;

public abstract class BaseControllerIntegrationTests : IAsyncLifetime
{
    protected readonly HttpClient AuthenticatedClient;
    protected readonly HttpClient Client;
    protected readonly ApiWebApplicationFactory Factory;

    protected BaseControllerIntegrationTests(ApiWebApplicationFactory factory)
    {
        Factory = factory;
        Client = Factory.CreateClient();
        AuthenticatedClient = CreateClientWithToken(TestJwt.Create());
    }

    public Task InitializeAsync() => ClearDatabase();

    public Task DisposeAsync() => Task.CompletedTask;

    protected HttpClient CreateClientWithToken(string token)
    {
        var client = Factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return client;
    }

    private async Task ClearDatabase()
    {
        using var scope = Factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<Database>();

        await context.TruncateAllTablesAsync();
        InvalidateCaches();
    }

    private void InvalidateCaches()
    {
        var cacheInvalidator = Factory.Services.GetRequiredService<CacheInvalidator>();
        cacheInvalidator.InvalidateProjects();
        cacheInvalidator.InvalidateCompanies();
        cacheInvalidator.InvalidateTags();
    }

    protected async Task<Project> CreateProjectAsync(
        ProjectId projectId,
        string title = "Test Project",
        CompanyId? companyId = null,
        List<string>? tags = null)
    {
        var startDate = new DateOnly(2021, 1, 1);
        var project = new Project
        {
            Id = projectId,
            Title = title,
            Description = "Test Description",
            ShortDescription = "Test Short Description",
            CompanyId = companyId,
            StartDate = startDate,
            EndDate = startDate.AddYears(1),
            Website = "https://example.com",
            Slug = title.ToSlug(),
            Tags = []
        };

        using var scope = Factory.Services.CreateScope();

        if (companyId is not null)
        {
            await CreateCompanyAsync(companyId.Value);
        }

        if (tags?.Count > 0)
        {
            var tagService = scope.ServiceProvider.GetRequiredService<ITagService>();
            var createdTags = await tagService.SyncTagsAsync(tags);
            if (createdTags.Value is not null) project.Tags = [.. createdTags.Value];
        }

        var context = scope.ServiceProvider.GetRequiredService<Database>();

        context.Project.Add(project);
        await context.SaveChangesAsync();
        InvalidateCaches();

        return project;
    }

    protected async Task<Tag> CreateTagAsync(
        TagId tagId,
        string title = "Test Tag",
        TagType type = TagType.Frontend,
        string? icon = null,
        string? customColour = null
    )
    {
        var tag = new Tag
        {
            Id = tagId,
            Title = title,
            Type = type,
            Icon = icon,
            CustomColour = customColour,
            Slug = title.ToSlug()
        };

        using var scope = Factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<Database>();

        context.Tag.Add(tag);
        await context.SaveChangesAsync();
        InvalidateCaches();

        return tag;
    }

    protected async Task<Company> CreateCompanyAsync(
        CompanyId companyId,
        string name = "Test Company",
        string description = "Test Description")
    {
        var company = new Company
        {
            Id = companyId,
            Name = name,
            Description = description,
            Website = "https://example.com",
            Slug = name.ToSlug()
        };

        using var scope = Factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<Database>();

        context.Company.Add(company);
        await context.SaveChangesAsync();
        InvalidateCaches();

        return company;
    }

    protected static StringContent CreateJsonContent<T>(T data) where T : class
    {
        return new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
    }

    protected static T? GetJsonContent<T>(string json)
    {
        return JsonConvert.DeserializeObject<T>(json);
    }

    protected static async Task AssertErrorAsync(HttpResponseMessage response, HttpStatusCode expectedStatus, string expectedErrorCode)
    {
        Assert.Equal(expectedStatus, response.StatusCode);
        var error = GetJsonContent<ApiErrorDto>(await response.Content.ReadAsStringAsync());
        Assert.NotNull(error);
        Assert.Equal(expectedErrorCode, error.Error);
    }

    protected static MultipartFormDataContent GetMultipartFormDataContent<T>(T data) where T : class
    {
        var content = new MultipartFormDataContent();

        foreach (var property in data.GetType().GetProperties())
        {
            var value = property.GetValue(data);
            if (value is null) continue;

            if (value is string stringValue)
                content.Add(new StringContent(stringValue), property.Name);
            else if (value is List<string> stringListValue)
                foreach (var item in stringListValue)
                    content.Add(new StringContent(item), property.Name);
            else if (value is IFormFile fileValue)
                content.Add(new StreamContent(fileValue.OpenReadStream()), property.Name, fileValue.FileName);
            else if (value is int intValue)
                content.Add(new StringContent(intValue.ToString()), property.Name);
            else if (value is DateOnly dateValue)
                content.Add(new StringContent(dateValue.ToString("yyyy-MM-dd")), property.Name);
            else if (value is IId idValue)
                content.Add(new StringContent(idValue.Value.ToString()), property.Name);
            else if (value is Guid guidValue)
                content.Add(new StringContent(guidValue.ToString()), property.Name);
            else
                content.Add(new StringContent(JsonConvert.SerializeObject(value)), property.Name);
        }

        return content;
    }
}
