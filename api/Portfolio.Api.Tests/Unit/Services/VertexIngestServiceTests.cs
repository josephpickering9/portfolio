using System.Text.Json.Nodes;
using Moq;
using Portfolio.Api.Models;
using Portfolio.Api.Services.VertexAi;
using Xunit;

namespace Portfolio.Api.Tests.Unit.Services;

public class VertexIngestServiceTests
{
    private readonly Mock<IVertexChatbotClient> _chatbotClient = new();
    private readonly VertexIngestService _vertexIngestService;

    public VertexIngestServiceTests()
    {
        _vertexIngestService = new VertexIngestService(_chatbotClient.Object);
    }

    [Fact]
    public async Task UpsertProjectAsync_ProjectWithCaseStudy_IncludesCaseStudyFieldsAndOrderedStats()
    {
        var project = new Project
        {
            Title = "Gigs",
            ShortDescription = "A gig tracker",
            Description = "<p>Tracks gigs</p>",
            ProblemStatement = "Spreadsheets don't show trends.",
            KeyDecision = "Generate the API client from OpenAPI.",
            Outcome = "A personal analytics dashboard.",
            Stats =
            [
                new ProjectStat { Label = "Years of history", Value = "17", Order = 1 },
                new ProjectStat { Label = "Gigs tracked", Value = "242", Order = 0 },
            ],
        };
        var dto = await CaptureUpsertedProject(project);

        Assert.Equal("Spreadsheets don't show trends.", dto.ProblemStatement);
        Assert.Equal("Generate the API client from OpenAPI.", dto.KeyDecision);
        Assert.Equal("A personal analytics dashboard.", dto.Outcome);
        Assert.Equal(["Gigs tracked: 242", "Years of history: 17"], dto.Stats);
    }

    [Fact]
    public async Task UpsertProjectAsync_ProjectWithoutCaseStudy_LeavesCaseStudyFieldsEmpty()
    {
        var project = new Project { Title = "Food Waste", ShortDescription = "An app", Description = "<p>An app</p>" };
        var dto = await CaptureUpsertedProject(project);

        Assert.Null(dto.ProblemStatement);
        Assert.Null(dto.KeyDecision);
        Assert.Null(dto.Outcome);
        Assert.NotNull(dto.Stats);
        Assert.Empty(dto.Stats);
    }

    [Theory]
    [InlineData(nameof(VertexProjectDto.ProblemStatement))]
    [InlineData(nameof(VertexProjectDto.KeyDecision))]
    [InlineData(nameof(VertexProjectDto.Outcome))]
    public void GenerateSchema_VertexProjectDto_CaseStudyTextFieldsAreSearchableAndOptional(string field)
    {
        var schema = JsonNode.Parse(VertexSchemaGenerator.GenerateSchema<VertexProjectDto>())!;
        var property = schema["properties"]![field]!;

        Assert.Equal("string", property["type"]!.GetValue<string>());
        Assert.True(property["searchable"]!.GetValue<bool>());
        Assert.True(property["retrievable"]!.GetValue<bool>());
        Assert.DoesNotContain(field, RequiredFields(schema));
    }

    [Fact]
    public void GenerateSchema_VertexProjectDto_StatsAreSearchableAndOptional()
    {
        var schema = JsonNode.Parse(VertexSchemaGenerator.GenerateSchema<VertexProjectDto>())!;
        var items = schema["properties"]![nameof(VertexProjectDto.Stats)]!["items"]!;

        Assert.Equal("string", items["type"]!.GetValue<string>());
        Assert.True(items["searchable"]!.GetValue<bool>());
        Assert.DoesNotContain(nameof(VertexProjectDto.Stats), RequiredFields(schema));
    }

    private static List<string> RequiredFields(JsonNode schema) =>
        schema["required"]!.AsArray().Select(n => n!.GetValue<string>()).ToList();

    private async Task<VertexProjectDto> CaptureUpsertedProject(Project project)
    {
        VertexProjectDto? captured = null;
        _chatbotClient
            .Setup(c => c.UpsertFeatureAsync(
                VertexFeatureType.Project,
                project.Id.ToString(),
                It.IsAny<VertexProjectDto>(),
                It.IsAny<string?>(),
                true,
                It.IsAny<CancellationToken>()))
            .Callback<VertexFeatureType, string, VertexProjectDto, string?, bool, CancellationToken>((_, _, value, _, _, _) => captured = value)
            .Returns(Task.CompletedTask);

        await _vertexIngestService.UpsertProjectAsync(project);

        Assert.NotNull(captured);
        return captured;
    }
}
