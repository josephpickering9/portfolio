using System.Net;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Portfolio.Api.Tests.Fixtures;
using Portfolio.Api.Types;
using Xunit;

namespace Portfolio.Api.Tests.Integration;

// Discovers every [Authorize] endpoint from the running app, so a new one is covered the moment it's added:
// none may answer an anonymous caller or a signed-in user without manage:content.
public class AuthorizationIntegrationTests(CustomWebApplicationFactory factory)
    : BaseControllerIntegrationTests(factory), IClassFixture<CustomWebApplicationFactory>
{
    [Fact]
    public void ProtectedRoutes_AreDiscovered()
    {
        Assert.NotEmpty(ProtectedRoutes());
    }

    [Fact]
    public async Task ProtectedRoutes_Anonymous_ReturnUnauthorized()
    {
        foreach (var (method, path) in ProtectedRoutes())
        {
            var response = await Client.SendAsync(new HttpRequestMessage(method, path));

            Assert.True(response.StatusCode == HttpStatusCode.Unauthorized, $"{method} {path} returned {response.StatusCode}");
            await AssertErrorAsync(response, HttpStatusCode.Unauthorized, ErrorCodes.Unauthorized);
        }
    }

    [Fact]
    public async Task ProtectedRoutes_TokenWithoutManageContent_ReturnForbidden()
    {
        var client = CreateClientWithToken(TestJwt.Create(permissions: []));

        foreach (var (method, path) in ProtectedRoutes())
        {
            var response = await client.SendAsync(new HttpRequestMessage(method, path));

            Assert.True(response.StatusCode == HttpStatusCode.Forbidden, $"{method} {path} returned {response.StatusCode}");
            await AssertErrorAsync(response, HttpStatusCode.Forbidden, ErrorCodes.Forbidden);
        }
    }

    private List<(HttpMethod Method, string Path)> ProtectedRoutes() =>
        Factory.Services.GetRequiredService<EndpointDataSource>().Endpoints
            .OfType<RouteEndpoint>()
            .Where(endpoint => endpoint.Metadata.GetMetadata<IAuthorizeData>() is not null
                               && endpoint.Metadata.GetMetadata<IAllowAnonymous>() is null)
            .SelectMany(endpoint => (endpoint.Metadata.GetMetadata<IHttpMethodMetadata>()?.HttpMethods ?? [])
                .Select(method => (new HttpMethod(method), "/" + FillRouteValues(endpoint.RoutePattern.RawText!))))
            .ToList();

    private static string FillRouteValues(string template) =>
        Regex.Replace(template, @"\{[^}]+\}", Guid.NewGuid().ToString());
}
