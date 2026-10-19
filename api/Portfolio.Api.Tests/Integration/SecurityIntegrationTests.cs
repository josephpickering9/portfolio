using System.Net;
using Portfolio.Api.Tests.Fixtures;
using Xunit;

namespace Portfolio.Api.Tests.Integration;

public class SecurityIntegrationTests(CustomWebApplicationFactory factory)
    : BaseControllerIntegrationTests(factory), IClassFixture<CustomWebApplicationFactory>
{
    private const string AllowedOrigin = "http://localhost:3000";
    private const string DisallowedOrigin = "https://not-the-frontend.example";

    [Fact]
    public async Task ApiResponses_CarrySecurityHeaders()
    {
        var response = await Client.GetAsync("/project");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("nosniff", Header(response, "X-Content-Type-Options"));
        Assert.Equal("DENY", Header(response, "X-Frame-Options"));
        Assert.Equal("no-referrer", Header(response, "Referrer-Policy"));
        Assert.Equal("same-site", Header(response, "Cross-Origin-Resource-Policy"));
        Assert.Equal("default-src 'none'; frame-ancestors 'none'", Header(response, "Content-Security-Policy"));
    }

    [Fact]
    public async Task Swagger_IsPublicAndExemptFromTheApiContentSecurityPolicy()
    {
        var response = await Client.GetAsync("/swagger/v1/swagger.json");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Null(Header(response, "Content-Security-Policy"));
    }

    [Fact]
    public async Task Cors_AllowsTheConfiguredFrontendOrigin()
    {
        var response = await Client.SendAsync(RequestFrom(AllowedOrigin));

        Assert.Equal(AllowedOrigin, Header(response, "Access-Control-Allow-Origin"));
    }

    [Fact]
    public async Task Cors_DoesNotAllowOtherOrigins()
    {
        var response = await Client.SendAsync(RequestFrom(DisallowedOrigin));

        Assert.Null(Header(response, "Access-Control-Allow-Origin"));
    }

    private static HttpRequestMessage RequestFrom(string origin)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "/project");
        request.Headers.Add("Origin", origin);
        return request;
    }

    private static string? Header(HttpResponseMessage response, string name) =>
        response.Headers.TryGetValues(name, out var values) || response.Content.Headers.TryGetValues(name, out values)
            ? string.Join(", ", values)
            : null;
}
