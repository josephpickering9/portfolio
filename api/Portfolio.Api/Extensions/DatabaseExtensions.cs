using Microsoft.EntityFrameworkCore;
using Portfolio.Api.Services;

namespace Portfolio.Api.Extensions;

public static class DatabaseExtensions
{
    private const string ConnectionStringName = "DefaultConnection";
    private const int CommandTimeoutSeconds = 300;
    private const string HealthCheckPath = "/health";

    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<Database>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString(ConnectionStringName),
                npgsql => npgsql.CommandTimeout(CommandTimeoutSeconds)));

        services.AddHealthChecks().AddDbContextCheck<Database>();
        services.AddMemoryCache();
        services.AddSingleton<CacheInvalidator>();

        return services;
    }

    public static WebApplication MapHealthEndpoint(this WebApplication app)
    {
        app.MapHealthChecks(HealthCheckPath);
        return app;
    }
}
