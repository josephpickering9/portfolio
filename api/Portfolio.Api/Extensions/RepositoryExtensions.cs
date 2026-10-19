using Portfolio.Api.Repositories;

namespace Portfolio.Api.Extensions;

public static class RepositoryExtensions
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IProjectRepository, ProjectRepository>();
        services.AddScoped<ICompanyRepository, CompanyRepository>();
        services.AddScoped<ITagRepository, TagRepository>();
        services.AddScoped<IProjectCodeRepository, ProjectCodeRepository>();
        services.AddScoped<IProjectImageRepository, ProjectImageRepository>();
        services.AddScoped<IProjectStatRepository, ProjectStatRepository>();

        return services;
    }
}
