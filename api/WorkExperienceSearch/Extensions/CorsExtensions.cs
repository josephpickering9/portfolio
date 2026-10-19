namespace Work_Experience_Search.Extensions;

public static class CorsExtensions
{
    private const string AllowAllPolicy = "AllowAll";

    public static IServiceCollection AddAllowAllCors(this IServiceCollection services) =>
        services.AddCors(options =>
            options.AddPolicy(AllowAllPolicy, policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

    public static WebApplication UseAllowAllCors(this WebApplication app)
    {
        app.UseCors(AllowAllPolicy);
        return app;
    }
}
