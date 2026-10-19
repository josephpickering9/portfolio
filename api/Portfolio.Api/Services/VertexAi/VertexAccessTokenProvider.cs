using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.Options;

namespace Portfolio.Api.Services.VertexAi;

public interface IVertexAccessTokenProvider
{
    Task<string> GetAccessTokenAsync(CancellationToken cancellationToken = default);
}

public sealed class VertexAccessTokenProvider(IOptions<VertexAiOptions> options, ILogger<VertexAccessTokenProvider> logger)
    : IVertexAccessTokenProvider
{
    private const string CloudPlatformScope = "https://www.googleapis.com/auth/cloud-platform";

    private readonly Lazy<GoogleCredential> _credential = new(
        () => VertexCredentialFactory.Create(options.Value, logger).CreateScoped(CloudPlatformScope),
        LazyThreadSafetyMode.PublicationOnly);

    public Task<string> GetAccessTokenAsync(CancellationToken cancellationToken = default) =>
        _credential.Value.UnderlyingCredential.GetAccessTokenForRequestAsync(cancellationToken: cancellationToken);
}
