using System.Net;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using Work_Experience_Search.Services.VertexAi;

namespace WorkExperienceSearchTests.Tests.Fixtures;

public sealed class VertexStubWebApplicationFactory : ApiWebApplicationFactory
{
    private Func<HttpRequestMessage, HttpResponseMessage> _respond = _ => Json(HttpStatusCode.OK, "{}");

    public Mock<IVertexChatbotClient> ChatbotClient { get; } = new();

    public void RespondWith(HttpStatusCode statusCode, string body) => _respond = _ => Json(statusCode, body);

    public void FailWith(Exception exception) => _respond = _ => throw exception;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<IVertexChatbotClient>();
            services.AddSingleton(ChatbotClient.Object);

            services.RemoveAll<IVertexAccessTokenProvider>();
            services.AddSingleton<IVertexAccessTokenProvider, StubAccessTokenProvider>();

            services.AddHttpClient<IVertexGenerateContentClient, VertexGenerateContentClient>()
                .ConfigurePrimaryHttpMessageHandler(() => new StubHttpMessageHandler(request => _respond(request)));
        });
    }

    private static HttpResponseMessage Json(HttpStatusCode statusCode, string body) =>
        new(statusCode) { Content = new StringContent(body, Encoding.UTF8, "application/json") };

    private sealed class StubAccessTokenProvider : IVertexAccessTokenProvider
    {
        public Task<string> GetAccessTokenAsync(CancellationToken cancellationToken = default) => Task.FromResult("test-token");
    }
}

public sealed class StubHttpMessageHandler(Func<HttpRequestMessage, HttpResponseMessage> respond) : HttpMessageHandler
{
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) =>
        Task.FromResult(respond(request));
}
