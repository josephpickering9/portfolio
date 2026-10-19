using System.Net;
using System.Text;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using Work_Experience_Search.Services.VertexAi;
using WorkExperienceSearchTests.Tests.Fixtures;
using Work_Experience_Search.Types;
using Xunit;

namespace WorkExperienceSearchTests.Tests.Unit.Services;

public class VertexGenerateContentClientTests
{
    private static readonly GenerateContentRequest Request = new(
        GenerateContentMessage.FromSystem("system"),
        [GenerateContentMessage.FromUser("hello")]);

    [Fact]
    public async Task GenerateContentAsync_SuccessfulResponse_ReturnsParsedCandidates()
    {
        var client = CreateClient(_ => Json(HttpStatusCode.OK, """{ "candidates": [ { "content": { "parts": [ { "text": "hi" } ] } } ] }"""));

        var result = (await client.GenerateContentAsync(Request)).ExpectSuccess();

        Assert.Equal("hi", result.Candidates.Single().Content!.Parts.Single().Text);
    }

    [Theory]
    [InlineData(HttpStatusCode.InternalServerError)]
    [InlineData(HttpStatusCode.TooManyRequests)]
    [InlineData(HttpStatusCode.Unauthorized)]
    public async Task GenerateContentAsync_NonSuccessStatus_ReturnsUpstreamUnavailable(HttpStatusCode statusCode)
    {
        var client = CreateClient(_ => Json(statusCode, """{ "error": "nope" }"""));

        var error = (await client.GenerateContentAsync(Request)).ExpectFailure();

        Assert.Equal(ErrorCodes.UpstreamUnavailable, error);
    }

    [Fact]
    public async Task GenerateContentAsync_NetworkFailure_ReturnsUpstreamUnavailable()
    {
        var client = CreateClient(_ => throw new HttpRequestException("connection refused"));

        var error = (await client.GenerateContentAsync(Request)).ExpectFailure();

        Assert.Equal(ErrorCodes.UpstreamUnavailable, error);
    }

    [Fact]
    public async Task GenerateContentAsync_MalformedBody_ReturnsUpstreamUnavailable()
    {
        var client = CreateClient(_ => Json(HttpStatusCode.OK, "not json"));

        var error = (await client.GenerateContentAsync(Request)).ExpectFailure();

        Assert.Equal(ErrorCodes.UpstreamUnavailable, error);
    }

    [Fact]
    public async Task GenerateContentAsync_SendsBearerTokenAndCamelCaseBody()
    {
        HttpRequestMessage? captured = null;
        string? capturedBody = null;
        var client = CreateClient(request =>
        {
            captured = request;
            capturedBody = request.Content!.ReadAsStringAsync().GetAwaiter().GetResult();
            return Json(HttpStatusCode.OK, "{}");
        });

        await client.GenerateContentAsync(Request);

        Assert.NotNull(captured);
        Assert.Equal("Bearer", captured.Headers.Authorization!.Scheme);
        Assert.Equal("test-token", captured.Headers.Authorization.Parameter);
        Assert.Contains("\"systemInstruction\"", capturedBody);
        Assert.Contains("\"contents\"", capturedBody);
        Assert.DoesNotContain("\"tools\"", capturedBody);
    }

    private static VertexGenerateContentClient CreateClient(Func<HttpRequestMessage, HttpResponseMessage> respond)
    {
        var tokenProvider = new Mock<IVertexAccessTokenProvider>();
        tokenProvider.Setup(p => p.GetAccessTokenAsync(It.IsAny<CancellationToken>())).ReturnsAsync("test-token");

        return new VertexGenerateContentClient(
            new HttpClient(new StubHttpMessageHandler(respond)),
            tokenProvider.Object,
            Options.Create(new VertexAiOptions { ProjectId = "test-project" }),
            NullLogger<VertexGenerateContentClient>.Instance);
    }

    private static HttpResponseMessage Json(HttpStatusCode statusCode, string body) =>
        new(statusCode) { Content = new StringContent(body, Encoding.UTF8, "application/json") };
}
