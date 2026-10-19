using System.Net;
using System.Net.Http.Json;
using Portfolio.Api.Tests.Fixtures;
using Portfolio.Api.Types;
using Xunit;

namespace Portfolio.Api.Tests.Integration;

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
