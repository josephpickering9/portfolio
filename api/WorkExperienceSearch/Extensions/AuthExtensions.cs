using Auth0.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Work_Experience_Search.Types;

namespace Work_Experience_Search.Extensions;

public static class AuthExtensions
{
    private const string DomainKey = "Auth0:Domain";
    private const string ClientIdKey = "Auth0:ClientId";
    private const string AudienceKey = "Auth0:Audience";

    public static IServiceCollection AddAuth0Authentication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthorization();
        services.AddAuth0WebAppAuthentication(options =>
        {
            options.Domain = configuration[DomainKey] ?? string.Empty;
            options.ClientId = configuration[ClientIdKey] ?? string.Empty;
        });

        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.Authority = configuration[DomainKey];
                options.Audience = configuration[AudienceKey];
                options.Events = new JwtBearerEvents
                {
                    OnChallenge = async context =>
                    {
                        context.HandleResponse();
                        context.Response.Headers.WWWAuthenticate = JwtBearerDefaults.AuthenticationScheme;
                        await context.Response.WriteErrorAsync(ErrorCodes.Unauthorized, context.HttpContext.RequestAborted);
                    },
                    OnForbidden = context =>
                        context.Response.WriteErrorAsync(ErrorCodes.Forbidden, context.HttpContext.RequestAborted),
                };
            });

        return services;
    }
}
