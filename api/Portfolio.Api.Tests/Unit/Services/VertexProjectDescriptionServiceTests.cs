using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Portfolio.Api.Repositories;
using Portfolio.Api.Requests;
using Portfolio.Api.Services;
using Portfolio.Api.Services.VertexAi;
using Portfolio.Api.Types;
using Xunit;

namespace Portfolio.Api.Tests.Unit.Services;

public class VertexProjectDescriptionServiceTests : BaseServiceTests
{
    private readonly Mock<IVertexGenerateContentClient> _generateContentClient = new();
    private readonly VertexProjectDescriptionService _descriptionService;

    public VertexProjectDescriptionServiceTests()
    {
        _descriptionService = new VertexProjectDescriptionService(
            _generateContentClient.Object,
            new ProjectRepository(Context, new MemoryCache(new MemoryCacheOptions()), new CacheInvalidator()),
            NullLogger<VertexProjectDescriptionService>.Instance);
    }

    [Fact]
    public async Task SuggestDescriptionAsync_UpstreamReturnsText_ReturnsTrimmedSuggestion()
    {
        RespondWith(Result<GenerateContentResponse>.Ok(TextResponse("  <div><p>Rewritten</p></div>\n")));

        var result = (await _descriptionService.SuggestDescriptionAsync(Project1Id, new SuggestProjectDescriptionRequest())).ExpectSuccess();

        Assert.Equal(Project1Id, result.ProjectId);
        Assert.Equal("Visit Northumberland", result.Title);
        Assert.Equal("<div><p>Rewritten</p></div>", result.SuggestedDescription);
    }

    [Fact]
    public async Task SuggestDescriptionAsync_NonExistingProject_ReturnsProjectNotFound()
    {
        var error = (await _descriptionService.SuggestDescriptionAsync(ProjectId.New(), new SuggestProjectDescriptionRequest())).ExpectFailure();

        Assert.Equal(ErrorCodes.ProjectNotFound, error);
        _generateContentClient.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task SuggestDescriptionAsync_UpstreamFails_ReturnsUpstreamUnavailable()
    {
        RespondWith(Result<GenerateContentResponse>.Fail(ErrorCodes.UpstreamUnavailable));

        var error = (await _descriptionService.SuggestDescriptionAsync(Project1Id, new SuggestProjectDescriptionRequest())).ExpectFailure();

        Assert.Equal(ErrorCodes.UpstreamUnavailable, error);
    }

    [Fact]
    public async Task SuggestDescriptionAsync_UpstreamReturnsNoText_ReturnsUpstreamEmptyResponse()
    {
        RespondWith(Result<GenerateContentResponse>.Ok(new GenerateContentResponse()));

        var error = (await _descriptionService.SuggestDescriptionAsync(Project1Id, new SuggestProjectDescriptionRequest())).ExpectFailure();

        Assert.Equal(ErrorCodes.UpstreamEmptyResponse, error);
    }

    [Fact]
    public async Task SuggestDescriptionAsync_PromptIncludesTargetWordRangeAndProjectData()
    {
        GenerateContentRequest? captured = null;
        _generateContentClient
            .Setup(c => c.GenerateContentAsync(It.IsAny<GenerateContentRequest>(), It.IsAny<CancellationToken>()))
            .Callback<GenerateContentRequest, CancellationToken>((request, _) => captured = request)
            .ReturnsAsync(Result<GenerateContentResponse>.Ok(TextResponse("<div/>")));

        await _descriptionService.SuggestDescriptionAsync(Project1Id, new SuggestProjectDescriptionRequest { TargetWordCount = 100 });

        Assert.NotNull(captured);
        var prompt = captured.Contents.Single().Parts.Single().Text;
        Assert.Contains("between 80 and 140 words", prompt);
        Assert.Contains("Visit Northumberland", prompt);
        Assert.Contains(Project1Id.Value.ToString(), prompt);
    }

    private static GenerateContentResponse TextResponse(string text) =>
        new() { Candidates = [new Candidate { Content = new Content { Parts = [new Part { Text = text }] } }] };

    private void RespondWith(Result<GenerateContentResponse> result) =>
        _generateContentClient
            .Setup(c => c.GenerateContentAsync(It.IsAny<GenerateContentRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);
}
