using Microsoft.AspNetCore.HttpOverrides;

namespace Portfolio.Api.Extensions;

public static class SecurityExtensions
{
    private const string SwaggerPath = "/swagger";
    private const string ApiContentSecurityPolicy = "default-src 'none'; frame-ancestors 'none'";
    private static readonly TimeSpan HstsMaxAge = TimeSpan.FromDays(365);

    // nginx terminates TLS and proxies over loopback: without this every request looks like
    // plain HTTP from 127.0.0.1, so HSTS is never sent and the rate limiter puts all clients in one partition.
    public static IServiceCollection AddReverseProxySupport(this IServiceCollection services) =>
        services
            .Configure<ForwardedHeadersOptions>(options =>
                options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto)
            .AddHsts(options => options.MaxAge = HstsMaxAge);

    public static WebApplication UseSecurityHeaders(this WebApplication app)
    {
        app.Use((context, next) =>
        {
            var headers = context.Response.Headers;
            headers.XContentTypeOptions = "nosniff";
            headers.XFrameOptions = "DENY";
            headers["Referrer-Policy"] = "no-referrer";
            headers["Cross-Origin-Resource-Policy"] = "same-site";
            if (!context.Request.Path.StartsWithSegments(SwaggerPath))
                headers.ContentSecurityPolicy = ApiContentSecurityPolicy;

            return next(context);
        });

        return app;
    }
}
