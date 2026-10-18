using System.Net.Http.Headers;
using System.Text.Json;
using Google.Apis.Auth.OAuth2.Responses;
using Microsoft.Extensions.Options;
using Work_Experience_Search.Types;

namespace Work_Experience_Search.Services.VertexAi;

public interface IVertexGenerateContentClient
{
    Task<Result<GenerateContentResponse>> GenerateContentAsync(GenerateContentRequest request, CancellationToken cancellationToken = default);
}

public sealed class VertexGenerateContentClient(
    HttpClient httpClient,
    IVertexAccessTokenProvider accessTokenProvider,
    IOptions<VertexAiOptions> options,
    ILogger<VertexGenerateContentClient> logger) : IVertexGenerateContentClient
{
    private const string BearerScheme = "Bearer";

    public async Task<Result<GenerateContentResponse>> GenerateContentAsync(GenerateContentRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var token = await accessTokenProvider.GetAccessTokenAsync(cancellationToken);

            using var message = new HttpRequestMessage(HttpMethod.Post, options.Value.GenerateContentUrl());
            message.Content = JsonContent.Create(request);
            message.Headers.Authorization = new AuthenticationHeaderValue(BearerScheme, token);

            using var response = await httpClient.SendAsync(message, cancellationToken);
            var body = await response.Content.ReadAsStringAsync(cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                logger.LogError("Vertex generateContent failed with {StatusCode}: {Body}", (int)response.StatusCode, body);
                return ErrorCodes.UpstreamUnavailable.ToFail();
            }

            var content = JsonSerializer.Deserialize<GenerateContentResponse>(body) ?? new GenerateContentResponse();
            return content.ToOk();
        }
        catch (Exception ex) when (IsUpstreamFailure(ex, cancellationToken))
        {
            logger.LogError(ex, "Vertex generateContent request could not be completed.");
            return ErrorCodes.UpstreamUnavailable.ToFail();
        }
    }

    private static bool IsUpstreamFailure(Exception exception, CancellationToken cancellationToken) =>
        exception is HttpRequestException or TokenResponseException or JsonException
        || (exception is TaskCanceledException && !cancellationToken.IsCancellationRequested);
}
