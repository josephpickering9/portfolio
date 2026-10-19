using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Portfolio.Api.Filters;

public sealed class SwaggerFileOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var formParameters = context.MethodInfo.GetParameters()
            .Where(p => p.GetCustomAttributes(typeof(FromFormAttribute), false).Length > 0
                        || p.ParameterType == typeof(IFormFile)
                        || p.ParameterType == typeof(IFormFileCollection)
                        || p.ParameterType == typeof(List<IFormFile>));

        var parameterInfos = formParameters.ToList();
        if (!parameterInfos.Any() || operation.RequestBody?.Content is not { } content)
            return;

        content.Clear();
        var schema = new OpenApiSchema { Properties = new Dictionary<string, IOpenApiSchema>() };

        foreach (var p in parameterInfos)
        {
            if (p.Name is not { } name) continue;

            schema.Properties.Add(name, ToFormSchema(p.ParameterType, context));
        }

        content.Add("multipart/form-data", new OpenApiMediaType
        {
            Schema = schema
        });
    }

    private static IOpenApiSchema ToFormSchema(Type parameterType, OperationFilterContext context)
    {
        if (parameterType == typeof(IFormFile))
            return new OpenApiSchema { Type = JsonSchemaType.String, Format = "binary" };

        if (parameterType == typeof(IFormCollection) || parameterType == typeof(List<IFormFile>))
            return new OpenApiSchema
            {
                Type = JsonSchemaType.Array,
                Items = new OpenApiSchema { Type = JsonSchemaType.String, Format = "binary" }
            };

        return context.SchemaGenerator.GenerateSchema(parameterType, context.SchemaRepository);
    }
}
