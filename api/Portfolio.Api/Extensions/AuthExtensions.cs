using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Portfolio.Api.Types;

namespace Portfolio.Api.Extensions;

public static class AuthExtensions
{
    private const string DomainKey = "Auth0:Domain";
    private const string AudienceKey = "Auth0:Audience";

    public const string PermissionsClaim = "permissions";
    public const string ManageContentPermission = "manage:content";

    public static IServiceCollection AddAuth0Authentication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthorizationBuilder()
            .SetDefaultPolicy(new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme)
                .RequireAuthenticatedUser()
                .RequireClaim(PermissionsClaim, ManageContentPermission)
                .Build());

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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
