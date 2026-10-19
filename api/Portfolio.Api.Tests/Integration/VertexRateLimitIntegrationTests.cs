using System.Net;
using System.Net.Http.Json;
using Portfolio.Api.Requests;
using Portfolio.Api.Tests.Fixtures;
using Portfolio.Api.Types;
using Xunit;

namespace Portfolio.Api.Tests.Integration;

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

    [Fact]
    public async Task Query_RateLimitIsPerForwardedClient()
    {
        factory.RespondWith(HttpStatusCode.OK, """{ "candidates": [] }""");

        for (var i = 0; i < PermitsPerWindow; i++)
        {
            var allowed = await Client.SendAsync(QueryFrom("203.0.113.10"));
            allowed.EnsureSuccessStatusCode();
        }

        var otherClient = await Client.SendAsync(QueryFrom("203.0.113.20"));

        Assert.Equal(HttpStatusCode.OK, otherClient.StatusCode);
    }

    private static HttpRequestMessage QueryFrom(string clientIp)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "/vertex/query")
        {
            Content = JsonContent.Create(new VertexQueryRequest("Per client"))
        };
        request.Headers.Add("X-Forwarded-For", clientIp);
        return request;
    }
}
