using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using Work_Experience_Search.Repositories;
using Work_Experience_Search.Services;
using Work_Experience_Search.Services.VertexAi;
using Work_Experience_Search.Types;
using Xunit;

namespace WorkExperienceSearchTests.Tests.Unit.Services;

public class VertexQueryServiceTests : BaseServiceTests
{
    private readonly Mock<IVertexGenerateContentClient> _generateContentClient = new();
    private readonly VertexQueryService _queryService;

    public VertexQueryServiceTests()
    {
        var cache = new MemoryCache(new MemoryCacheOptions());
        var cacheInvalidator = new CacheInvalidator();
        _queryService = new VertexQueryService(
            _generateContentClient.Object,
            Options.Create(new VertexAiOptions { ProjectId = "test-project", DataStoreVersion = "v3" }),
            new ProjectRepository(Context, cache, cacheInvalidator),
            new CompanyRepository(Context, cache, cacheInvalidator),
            new TagRepository(Context, cache, cacheInvalidator),
            NullLogger<VertexQueryService>.Instance);
    }

    [Fact]
    public async Task QueryAsync_UpstreamFails_ReturnsUpstreamUnavailable()
    {
        RespondWith(Result<GenerateContentResponse>.Fail(ErrorCodes.UpstreamUnavailable));

        var error = (await _queryService.QueryAsync("anything")).ExpectFailure();

        Assert.Equal(ErrorCodes.UpstreamUnavailable, error);
    }

    [Fact]
    public async Task QueryAsync_GroundedAnswer_ResolvesProjectCitation()
    {
        RespondWith(Result<GenerateContentResponse>.Ok(new GenerateContentResponse
        {
            Candidates =
            [
                new Candidate
                {
                    Content = new Content { Parts = [new Part { Text = "Joe built Visit Northumberland." }] },
                    GroundingMetadata = new GroundingMetadata
                    {
                        GroundingChunks =
                        [
                            new GroundingChunk
                            {
                                RetrievedContext = new RetrievedContext
                                {
                                    DocumentName = $"projects/p/locations/global/collections/c/dataStores/local_project_structured_v3/branches/0/documents/{Project1Id}"
                                }
                            }
                        ]
                    }
                }
            ]
        }));

        var result = (await _queryService.QueryAsync("What did Joe build?")).ExpectSuccess();

        Assert.Equal("Joe built Visit Northumberland.", result.Answer);
        var citation = Assert.Single(result.Citations);
        Assert.Equal(VertexFeatureType.Project, citation.FeatureType);
        Assert.Equal(Project1Id.Value, citation.Id);
        Assert.Equal("Visit Northumberland", citation.Title);
        Assert.NotNull(citation.Project);
    }

    [Fact]
    public async Task QueryAsync_EmptyAnswer_ReturnsFallbackAnswer()
    {
        RespondWith(Result<GenerateContentResponse>.Ok(new GenerateContentResponse()));

        var result = (await _queryService.QueryAsync("anything")).ExpectSuccess();

        Assert.Equal(VertexQueryService.FallbackAnswer, result.Answer);
        Assert.Empty(result.Citations);
    }

    [Fact]
    public async Task QueryAsync_TargetsProjectDataStore()
    {
        GenerateContentRequest? captured = null;
        _generateContentClient
            .Setup(c => c.GenerateContentAsync(It.IsAny<GenerateContentRequest>(), It.IsAny<CancellationToken>()))
            .Callback<GenerateContentRequest, CancellationToken>((request, _) => captured = request)
            .ReturnsAsync(Result<GenerateContentResponse>.Ok(new GenerateContentResponse()));

        await _queryService.QueryAsync("anything");

        Assert.NotNull(captured);
        var datastore = Assert.Single(captured.Tools!).Retrieval.VertexAiSearch.Datastore;
        Assert.Equal("projects/test-project/locations/global/collections/default_collection/dataStores/local_project_structured_v3", datastore);
    }

    private void RespondWith(Result<GenerateContentResponse> result) =>
        _generateContentClient
            .Setup(c => c.GenerateContentAsync(It.IsAny<GenerateContentRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);
}
