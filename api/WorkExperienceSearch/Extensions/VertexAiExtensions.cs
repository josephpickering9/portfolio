using Work_Experience_Search.Services.VertexAi;

namespace Work_Experience_Search.Extensions;

public static class VertexAiExtensions
{
    public static IServiceCollection AddVertexAi(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<VertexAiOptions>(configuration.GetSection(VertexAiOptions.SectionName));

        services.AddSingleton<IVertexChatbotClient, VertexChatbotClient>();
        services.AddSingleton<IVertexAccessTokenProvider, VertexAccessTokenProvider>();
        services.AddHttpClient<IVertexGenerateContentClient, VertexGenerateContentClient>();

        services.AddScoped<IVertexIngestService, VertexIngestService>();
        services.AddScoped<IVertexIngestOrchestrator, VertexIngestOrchestrator>();
        services.AddScoped<IVertexQueryService, VertexQueryService>();
        services.AddScoped<IVertexProjectDescriptionService, VertexProjectDescriptionService>();

        return services;
    }
}
