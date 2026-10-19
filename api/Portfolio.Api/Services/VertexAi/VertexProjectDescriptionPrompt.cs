using System.Text.Json;
using System.Text.Json.Serialization;
using Portfolio.Api.Models;
using Portfolio.Api.Requests;
using Portfolio.Api.Types;

namespace Portfolio.Api.Services.VertexAi;

internal static class VertexProjectDescriptionPrompt
{
    private const int DefaultTargetWordCount = 170;
    private const int MinimumWordCount = 80;
    private const int WordCountTolerance = 40;
    private const string NoneSupplied = "None supplied.";

    public const string SystemPrompt = """
        You rewrite portfolio project descriptions for a personal developer site.

        Your goals:
        - Make each blurb concise, engaging, and focused on impact.
        - Highlight the developer's role, key features, and notable technical decisions.
        - Use first-person singular ("I") and past tense.
        - Weave technologies naturally into the narrative.
        - Prefer specific, concrete achievements (performance, usability, scale, automation) over vague claims.
        - Avoid corporate jargon and filler phrases like "comprehensive platform", "stands as a testament", or "user-centric digital solutions".
        - Do not invent features or metrics that are not supported by the input data.

        Output format:
        - Return a valid HTML snippet only (no <html>, <head>, or <body> tags).
        - Wrap the content in a single root <div>.
        - Inside, use one or two <p> elements for the summary, followed by a <ul> with 2–4 <li> items.
        - Use simple semantic tags (<p>, <ul>, <li>, <strong>, <span>) and optional Tailwind / daisyUI utility classes such as "space-y-3", "text-sm", or "list-disc list-inside".
        - Do not include a title or heading element (no <h1>–<h6>).
        - Return raw HTML only; do NOT wrap the response in markdown or code fences.
        """;

    private static readonly JsonSerializerOptions PromptSerializerOptions = new()
    {
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters = { new IdJsonConverterFactory(), new JsonStringEnumConverter() }
    };

    public static string Build(Project project, SuggestProjectDescriptionRequest request)
    {
        var targetWordCount = request.TargetWordCount is > 0 ? request.TargetWordCount.Value : DefaultTargetWordCount;
        var lowerBound = Math.Max(MinimumWordCount, targetWordCount - WordCountTolerance);
        var upperBound = targetWordCount + WordCountTolerance;

        var projectJson = JsonSerializer.Serialize(ProjectPromptData.From(project), PromptSerializerOptions);
        var extraContext = string.IsNullOrWhiteSpace(request.AdditionalContext) ? NoneSupplied : request.AdditionalContext.Trim();
        var additionalData = request.AdditionalData is { Count: > 0 }
            ? JsonSerializer.Serialize(request.AdditionalData, PromptSerializerOptions)
            : NoneSupplied;

        return $"""
            Rewrite the project description for my personal portfolio site.

            Requirements:
            - Length: Aim for between {lowerBound} and {upperBound} words of visible text.
            - Start with 1–2 sentences wrapped in <p> elements explaining what the product is, who it is for, and the main impact.
            - After the summary paragraphs, include a <ul> with 2–4 <li> items focusing on:
              - What I personally did on the project.
              - 2–4 key features or technical achievements that made a difference.
              - How and where the main technologies were used.
            - Use first-person singular ("I") and past tense.
            - Use natural, straightforward language; avoid corporate buzzwords.
            - The response MUST be HTML that can be rendered directly inside a Tailwind / daisyUI page. Do not include <html>, <head>, <body>, or a title element.
            - Do not invent features, metrics, or company claims that are not in the data.

            Here is the structured project data (source of truth – you may pull details from any of these fields, but do not hallucinate):
            {projectJson}

            Additional context from me:
            {extraContext}

            Additional structured data (may be empty):
            {additionalData}
            """;
    }

    private sealed record ProjectPromptData
    {
        public required ProjectId Id { get; init; }
        public required string Title { get; init; }
        public required string ShortDescription { get; init; }
        public required string Description { get; init; }
        public required DateOnly StartDate { get; init; }
        public DateOnly? EndDate { get; init; }
        public string? Website { get; init; }
        public bool ShowMockup { get; init; }
        public CompanyPromptData? Company { get; init; }
        public required IReadOnlyList<TagPromptData> Tags { get; init; }
        public required IReadOnlyList<RepositoryPromptData> Repositories { get; init; }
        public required IReadOnlyList<ImagePromptData> Images { get; init; }

        public static ProjectPromptData From(Project project) =>
            new()
            {
                Id = project.Id,
                Title = project.Title,
                ShortDescription = project.ShortDescription,
                Description = project.Description,
                StartDate = project.StartDate,
                EndDate = project.EndDate,
                Website = project.Website,
                ShowMockup = project.ShowMockup,
                Company = project.Company is null ? null : CompanyPromptData.From(project.Company),
                Tags = project.Tags.Select(t => new TagPromptData(t.Title, t.Type.ToString(), t.Icon, t.CustomColour)).ToList(),
                Repositories = project.Repositories.Select(r => new RepositoryPromptData(r.Title, r.Url, r.Order)).ToList(),
                Images = project.Images.Select(i => new ImagePromptData(i.Type, i.Order, i.Image)).ToList()
            };
    }

    private sealed record CompanyPromptData(CompanyId Id, string Name, string Description, string? Website, DateOnly? StartDate, DateOnly? EndDate)
    {
        public static CompanyPromptData From(Company company) =>
            new(company.Id, company.Name, company.Description, company.Website, company.StartDate, company.EndDate);
    }

    private sealed record TagPromptData(string Title, string Type, string? Icon, string? CustomColour);

    private sealed record RepositoryPromptData(string Title, string Url, int? Order);

    private sealed record ImagePromptData(ImageType Type, int? Order, string Image);
}
