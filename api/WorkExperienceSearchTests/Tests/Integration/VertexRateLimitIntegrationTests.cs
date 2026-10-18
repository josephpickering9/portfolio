using System.Net;
using System.Net.Http.Json;
using Work_Experience_Search.Requests;
using Work_Experience_Search.Tests;
using Work_Experience_Search.Types;
using Xunit;

namespace WorkExperienceSearchTests.Tests.Integration;

[Collection("Sequential")]
public class VertexRateLimitIntegrationTests(VertexStubWebApplicationFactory factory)
    : BaseControllerIntegrationTests(factory), IClassFixture<VertexStubWebApplicationFactory>
{
    private const int PermitsPerWindow = 10;

    [Fact]
    public async Task Query_ExceedingRateLimit_ReturnsRateLimited()
    {
        factory.RespondWith(HttpStatusCode.OK, """{ "candidates": [] }""");

        for (var i = 0; i < PermitsPerWindow; i++)
        {
            var allowed = await Client.PostAsJsonAsync("/vertex/query", new VertexQueryRequest("Within limit"));
            allowed.EnsureSuccessStatusCode();
        }

        var rejected = await Client.PostAsJsonAsync("/vertex/query", new VertexQueryRequest("Over limit"));

        await AssertErrorAsync(rejected, HttpStatusCode.TooManyRequests, ErrorCodes.RateLimited);
    }
}
