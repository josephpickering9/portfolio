using Work_Experience_Search.Services;
using Work_Experience_Search.Services.Image;

namespace Work_Experience_Search.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IFileService, FileService>();
        services.AddScoped<IImageService, ImageService>();
        services.AddScoped<IProjectService, ProjectService>();
        services.AddScoped<IProjectImageService, ProjectImageService>();
        services.AddScoped<IProjectRepositoryService, ProjectRepositoryService>();
        services.AddScoped<IProjectStatService, ProjectStatService>();
        services.AddScoped<ITagService, TagService>();
        services.AddScoped<ICompanyService, CompanyService>();

        return services;
    }
}
