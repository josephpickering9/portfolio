using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;
using Work_Experience_Search.Types;

namespace Work_Experience_Search.Extensions;

public static class RateLimitingExtensions
{
    private const int VertexQueryPermitsPerWindow = 10;
    private static readonly TimeSpan VertexQueryWindow = TimeSpan.FromMinutes(1);
    private const string UnknownClientPartition = "unknown";

    // Each Vertex query is a paid call and the endpoint is public, so cap it per client IP.
    public static IServiceCollection AddRateLimitPolicies(this IServiceCollection services) =>
        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            options.OnRejected = (context, cancellationToken) =>
                new ValueTask(context.HttpContext.Response.WriteErrorAsync(ErrorCodes.RateLimited, cancellationToken));
            options.AddPolicy(RateLimitPolicies.VertexQuery, context => RateLimitPartition.GetFixedWindowLimiter(
                context.Connection.RemoteIpAddress?.ToString() ?? UnknownClientPartition,
                _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = VertexQueryPermitsPerWindow,
                    Window = VertexQueryWindow,
                    QueueLimit = 0,
                }));
        });
}
