using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;
using System;

namespace MessagingService.Common;

/// <summary>
/// Adds DTO XML documentation comments to generated OpenAPI schemas and properties.
/// </summary>
internal sealed class DtoXmlDocumentationSchemaTransformer : IOpenApiSchemaTransformer
{
    private readonly IReadOnlyDictionary<string, string> summaries;

    public DtoXmlDocumentationSchemaTransformer()
    {
        string documentationPath = Path.Combine(AppContext.BaseDirectory, "MessagingService.DataTransferObjects.xml");
        this.summaries = LoadSummaries(documentationPath);
    }

    public Task TransformAsync(OpenApiSchema schema,
                               OpenApiSchemaTransformerContext context,
                               CancellationToken cancellationToken)
    {
        if (context.JsonPropertyInfo?.AttributeProvider is PropertyInfo property)
        {
            string propertyMemberName = $"P:{property.DeclaringType?.FullName}.{property.Name}";
            if (this.summaries.TryGetValue(propertyMemberName, out string? propertySummary))
            {
                schema.Description = propertySummary;
            }
        }

        Type? schemaType = context.JsonTypeInfo?.Type;
        if (schemaType is not null)
        {
            string typeMemberName = $"T:{schemaType.FullName}";
            if (this.summaries.TryGetValue(typeMemberName, out string? typeSummary))
            {
                schema.Description ??= typeSummary;
            }
        }

        return Task.CompletedTask;
    }

    private static IReadOnlyDictionary<string, string> LoadSummaries(string documentationPath)
    {
        if (File.Exists(documentationPath) == false)
        {
            return new Dictionary<string, string>();
        }

        XDocument document = XDocument.Load(documentationPath);
        return document.Descendants("member")
            .Select(member => new
            {
                Name = (string?)member.Attribute("name"),
                Summary = member.Element("summary")?.Value.Trim()
            })
            .Where(member => string.IsNullOrWhiteSpace(member.Name) == false
                             && string.IsNullOrWhiteSpace(member.Summary) == false)
            .ToDictionary(member => member.Name!, member => member.Summary!);
    }
}
