using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;

namespace Work_Experience_Search.Types;

public static class RateLimitPolicies
{
    public const string VertexQuery = "vertex-query";

    private const int VertexQueryPermitsPerWindow = 10;
    private static readonly TimeSpan VertexQueryWindow = TimeSpan.FromMinutes(1);

    // Each Vertex query is a paid call and the endpoint is public, so cap it per client IP.
    public static IServiceCollection AddRateLimitPolicies(this IServiceCollection services) =>
        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            options.AddPolicy(VertexQuery, context => RateLimitPartition.GetFixedWindowLimiter(
                context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = VertexQueryPermitsPerWindow,
                    Window = VertexQueryWindow,
                    QueueLimit = 0,
                }));
        });
}
