using Microsoft.OpenApi.Models;
using Work_Experience_Search.Filters;
using Work_Experience_Search.Types;

namespace Work_Experience_Search.Extensions;

public static class SwaggerExtensions
{
    private const string DocumentName = "v1";
    private const string DocumentTitle = "Work Experience API";

    public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc(DocumentName, new OpenApiInfo { Title = DocumentTitle, Version = DocumentName });

            options.OperationFilter<SwaggerFileOperationFilter>();
            options.SchemaFilter<EnumDescriptionSchemaFilter>();
            options.SupportNonNullableReferenceTypes();

            foreach (var idType in StronglyTypedIdTypes())
                options.MapType(idType, () => new OpenApiSchema { Type = "string", Format = "uuid" });
        });

        return services;
    }

    public static WebApplication UseSwaggerDocumentation(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI();
        return app;
    }

    private static IEnumerable<Type> StronglyTypedIdTypes() =>
        typeof(IId).Assembly.GetTypes().Where(type => type.IsValueType && typeof(IId).IsAssignableFrom(type));
}
