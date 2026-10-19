namespace Portfolio.Api.Extensions;

public static class CorsExtensions
{
    private const string FrontendPolicy = "Frontend";
    private const string AllowedOriginsKey = "Cors:AllowedOrigins";

    public static IServiceCollection AddFrontendCors(this IServiceCollection services, IConfiguration configuration)
    {
        var allowedOrigins = configuration.GetSection(AllowedOriginsKey).Get<string[]>() ?? [];

        return services.AddCors(options =>
            options.AddPolicy(FrontendPolicy, policy => policy.WithOrigins(allowedOrigins).AllowAnyMethod().AllowAnyHeader()));
    }

    public static WebApplication UseFrontendCors(this WebApplication app)
    {
        app.UseCors(FrontendPolicy);
        return app;
    }
}
