using System.Net;
using System.Net.Http.Json;
using Work_Experience_Search.Tests;
using Work_Experience_Search.Types;
using Xunit;

namespace WorkExperienceSearchTests.Tests.Integration;

[Collection("Sequential")]
public class VertexControllerIntegrationTests(CustomWebApplicationFactory customWebApplicationFactory)
    : BaseControllerIntegrationTests(customWebApplicationFactory), IClassFixture<CustomWebApplicationFactory>
{
    [Fact]
    public async Task PostIngest_Unauthorized_ReturnsUnauthorized()
    {
        var httpResponse = await Client.PostAsync("/vertex/ingest", null);

        await AssertErrorAsync(httpResponse, HttpStatusCode.Unauthorized, ErrorCodes.Unauthorized);
    }

    [Fact]
    public async Task PostSuggestDescription_Unauthorized_ReturnsUnauthorized()
    {
        var httpResponse = await Client.PostAsJsonAsync($"/vertex/projects/{Guid.NewGuid()}/description/suggest", new { });

        await AssertErrorAsync(httpResponse, HttpStatusCode.Unauthorized, ErrorCodes.Unauthorized);
    }
}
