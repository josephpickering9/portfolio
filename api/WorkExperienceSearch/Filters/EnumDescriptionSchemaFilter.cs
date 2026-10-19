using System.ComponentModel;
using System.Reflection;
using System.Text.Json.Nodes;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Work_Experience_Search.Filters;

public sealed class EnumDescriptionSchemaFilter : ISchemaFilter
{
    public void Apply(IOpenApiSchema schema, SchemaFilterContext context)
    {
        if (!context.Type.IsEnum || schema is not OpenApiSchema enumSchema) return;

        var enumDescriptions = new List<string>();
        foreach (var enumValue in Enum.GetValues(context.Type))
        {
            var value = enumValue.ToString();
            if (string.IsNullOrEmpty(value)) continue;

            var memberInfo = context.Type.GetMember(value).FirstOrDefault();
            var descriptionAttribute = memberInfo?.GetCustomAttribute<DescriptionAttribute>();
            enumDescriptions.Add(descriptionAttribute?.Description ?? value);
        }

        enumSchema.Enum = enumDescriptions.Select(JsonNode (description) => JsonValue.Create(description)).ToList();
        enumSchema.Type = JsonSchemaType.String;
    }
}
