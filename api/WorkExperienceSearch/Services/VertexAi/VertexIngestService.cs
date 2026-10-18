using System.Text.RegularExpressions;
using Work_Experience_Search.Models;

namespace Work_Experience_Search.Services.VertexAi;

public interface IVertexIngestService
{
    Task UpsertProjectAsync(Project project, CancellationToken cancellationToken = default);
    Task UpsertCompanyAsync(Company company, CancellationToken cancellationToken = default);
    Task UpsertTagAsync(Tag tag, CancellationToken cancellationToken = default);
}

public sealed partial class VertexIngestService(IVertexChatbotClient chatbotClient) : IVertexIngestService
{
    public Task UpsertProjectAsync(Project project, CancellationToken cancellationToken = default) =>
        chatbotClient.UpsertFeatureAsync(
            featureType: VertexFeatureType.Project,
            documentId: project.Id.ToString(),
            value: FlattenProject(project),
            jsonSchema: VertexSchemaGenerator.GenerateSchema<VertexProjectDto>(),
            ensureSchema: true,
            cancellationToken: cancellationToken);

    public Task UpsertCompanyAsync(Company company, CancellationToken cancellationToken = default) =>
        chatbotClient.UpsertFeatureAsync(
            featureType: VertexFeatureType.Company,
            documentId: company.Id.ToString(),
            value: FlattenCompany(company),
            jsonSchema: VertexSchemaGenerator.GenerateSchema<VertexCompanyDto>(),
            ensureSchema: true,
            cancellationToken: cancellationToken);

    public Task UpsertTagAsync(Tag tag, CancellationToken cancellationToken = default) =>
        chatbotClient.UpsertFeatureAsync(
            featureType: VertexFeatureType.Tag,
            documentId: tag.Id.ToString(),
            value: FlattenTag(tag),
            jsonSchema: VertexSchemaGenerator.GenerateSchema<VertexTagDto>(),
            ensureSchema: true,
            cancellationToken: cancellationToken);

    [GeneratedRegex("<[^>]*>")]
    private static partial Regex HtmlTagRegex();

    private static string StripHtml(string html) =>
        HtmlTagRegex().Replace(html, " ").Replace("&amp;", "&").Replace("&lt;", "<").Replace("&gt;", ">").Replace("&nbsp;", " ").Trim();

    private static VertexProjectDto FlattenProject(Project project) =>
        new()
        {
            Id = project.Id,
            Title = project.Title,
            ShortDescription = project.ShortDescription,
            Description = StripHtml(project.Description),
            ProblemStatement = project.ProblemStatement,
            KeyDecision = project.KeyDecision,
            Outcome = project.Outcome,
            Stats = project.Stats.OrderBy(s => s.Order ?? 0).Select(s => $"{s.Label}: {s.Value}").ToList(),
            StartDate = project.StartDate,
            EndDate = project.EndDate,
            Website = project.Website,
            Company = project.Company?.Name,
            CompanyId = project.CompanyId,
            Tags = project.Tags.Select(t => t.Title).ToList(),
            TagTypes = project.Tags.Select(t => t.Type.ToString()).ToList(),
        };

    private static VertexCompanyDto FlattenCompany(Company company) =>
        new()
        {
            Id = company.Id,
            Name = company.Name,
            Description = StripHtml(company.Description),
            Website = company.Website,
            StartDate = company.StartDate,
            EndDate = company.EndDate
        };

    private static VertexTagDto FlattenTag(Tag tag) =>
        new()
        {
            Title = tag.Title,
            Type = tag.Type.ToString()
        };
}
