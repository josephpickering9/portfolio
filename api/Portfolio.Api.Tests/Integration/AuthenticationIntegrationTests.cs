using System.Net;
using Portfolio.Api.Types;
using Portfolio.Api.Tests.Fixtures;
using Xunit;

namespace Portfolio.Api.Tests.Integration;

public class AuthenticationIntegrationTests(CustomWebApplicationFactory customWebApplicationFactory)
    : BaseControllerIntegrationTests(customWebApplicationFactory), IClassFixture<CustomWebApplicationFactory>
{
    [Fact]
    public async Task ProtectedEndpoint_ValidToken_ReachesController()
    {
        var httpResponse = await AuthenticatedClient.DeleteAsync($"/tag/{TagId.New()}");

        await AssertErrorAsync(httpResponse, HttpStatusCode.NotFound, ErrorCodes.TagNotFound);
    }

    [Fact]
    public async Task ProtectedEndpoint_ExpiredToken_ReturnsUnauthorized()
    {
        var client = CreateClientWithToken(TestJwt.Create(expires: DateTime.UtcNow.AddMinutes(-10)));

        var httpResponse = await client.DeleteAsync($"/tag/{TagId.New()}");

        await AssertErrorAsync(httpResponse, HttpStatusCode.Unauthorized, ErrorCodes.Unauthorized);
    }

    [Fact]
    public async Task ProtectedEndpoint_TokenSignedWithAnotherKey_ReturnsUnauthorized()
    {
        var client = CreateClientWithToken(TestJwt.Create(signingKey: TestJwt.CreateKey("some-other-signing-key-0123456789abcdef")));

        var httpResponse = await client.DeleteAsync($"/tag/{TagId.New()}");

        await AssertErrorAsync(httpResponse, HttpStatusCode.Unauthorized, ErrorCodes.Unauthorized);
    }

    [Fact]
    public async Task ProtectedEndpoint_TokenForAnotherAudience_ReturnsUnauthorized()
    {
        var client = CreateClientWithToken(TestJwt.Create(audience: "another-api"));

        var httpResponse = await client.DeleteAsync($"/tag/{TagId.New()}");

        await AssertErrorAsync(httpResponse, HttpStatusCode.Unauthorized, ErrorCodes.Unauthorized);
    }
}
