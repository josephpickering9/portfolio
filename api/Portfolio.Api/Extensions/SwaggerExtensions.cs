using Microsoft.OpenApi;
using Portfolio.Api.Filters;
using Portfolio.Api.Types;

namespace Portfolio.Api.Extensions;

public static class SwaggerExtensions
{
    private const string DocumentName = "v1";
    private const string DocumentTitle = "Portfolio API";

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
                options.MapType(idType, () => new OpenApiSchema { Type = JsonSchemaType.String, Format = "uuid" });
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
