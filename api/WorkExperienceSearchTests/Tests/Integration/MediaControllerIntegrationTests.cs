using System.Net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Work_Experience_Search.Extensions;
using WorkExperienceSearchTests.Tests.Fixtures;
using Work_Experience_Search.Types;
using Xunit;

namespace WorkExperienceSearchTests.Tests.Integration;

public class MediaControllerIntegrationTests(CustomWebApplicationFactory customWebApplicationFactory)
    : BaseControllerIntegrationTests(customWebApplicationFactory), IClassFixture<CustomWebApplicationFactory>
{
    [Fact]
    public async Task GetFile_ExistingFile_ReturnsFile()
    {
        const string fileName = "testfile.txt";
        const string testContent = "This is a test file.";
        var webHostEnvironment = Factory.Services.GetRequiredService<IWebHostEnvironment>();
        var uploadDirectory = webHostEnvironment.GetUploadDirectory();
        var filePath = Path.Combine(uploadDirectory, fileName);
        Directory.CreateDirectory(uploadDirectory);
        await File.WriteAllTextAsync(filePath, testContent);

        var response = await Client.GetAsync($"/media/uploads/{fileName}");

        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        Assert.Equal(testContent, content);

        File.Delete(filePath);
    }

    [Fact]
    public async Task GetFile_NonExistingFile_ReturnsNotFound()
    {
        const string nonExistingFileName = "nonexistingfile.txt";

        var response = await Client.GetAsync($"/media/uploads/{nonExistingFileName}");

        await AssertErrorAsync(response, HttpStatusCode.NotFound, ErrorCodes.FileNotFound);
    }
}