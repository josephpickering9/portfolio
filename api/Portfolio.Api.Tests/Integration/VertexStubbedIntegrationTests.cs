using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Grpc.Core;
using Moq;
using Portfolio.Api.Requests;
using Portfolio.Api.Services.VertexAi;
using Portfolio.Api.Tests.Fixtures;
using Portfolio.Api.Types;
using Xunit;

namespace Portfolio.Api.Tests.Integration;

public class VertexStubbedIntegrationTests(VertexStubWebApplicationFactory factory)
    : BaseControllerIntegrationTests(factory), IClassFixture<VertexStubWebApplicationFactory>
{
    private const string QueryAnswer = "Joe built the Test Project.";

    [Fact]
    public async Task Query_UpstreamSucceeds_ReturnsAnswerWithResolvedCitation()
    {
        var project = await CreateProjectAsync(ProjectId.New(), title: "Cited Project");
        factory.RespondWith(HttpStatusCode.OK, GroundedAnswer(QueryAnswer, project.Id));

        var httpResponse = await Client.PostAsJsonAsync("/vertex/query", new VertexQueryRequest("What did Joe build?"));

        httpResponse.EnsureSuccessStatusCode();
        var response = GetJsonContent<QueryResponse>(await httpResponse.Content.ReadAsStringAsync());
        Assert.NotNull(response);
        Assert.Equal(QueryAnswer, response.Answer);
        var citation = Assert.Single(response.Citations);
        Assert.Equal(project.Id.Value, citation.Id);
        Assert.Equal("Project", citation.FeatureType);
        Assert.Equal("Cited Project", citation.Title);
    }

    [Fact]
    public async Task Query_UpstreamReturnsNoCandidates_ReturnsFallbackAnswer()
    {
        factory.RespondWith(HttpStatusCode.OK, """{ "candidates": [] }""");

        var httpResponse = await Client.PostAsJsonAsync("/vertex/query", new VertexQueryRequest("Anything?"));

        httpResponse.EnsureSuccessStatusCode();
        var response = GetJsonContent<QueryResponse>(await httpResponse.Content.ReadAsStringAsync());
        Assert.NotNull(response);
        Assert.Equal(VertexQueryService.FallbackAnswer, response.Answer);
        Assert.Empty(response.Citations);
    }

    [Fact]
    public async Task Query_UpstreamReturnsServerError_ReturnsUpstreamUnavailable()
    {
        factory.RespondWith(HttpStatusCode.InternalServerError, """{ "error": { "message": "boom" } }""");

        var httpResponse = await Client.PostAsJsonAsync("/vertex/query", new VertexQueryRequest("What did Joe build?"));

        await AssertErrorAsync(httpResponse, HttpStatusCode.BadGateway, ErrorCodes.UpstreamUnavailable);
    }

    [Fact]
    public async Task Query_UpstreamUnreachable_ReturnsUpstreamUnavailable()
    {
        factory.FailWith(new HttpRequestException("connection refused"));

        var httpResponse = await Client.PostAsJsonAsync("/vertex/query", new VertexQueryRequest("What did Joe build?"));

        await AssertErrorAsync(httpResponse, HttpStatusCode.BadGateway, ErrorCodes.UpstreamUnavailable);
    }

    [Fact]
    public async Task Query_EmptyQuery_ReturnsValidationCode()
    {
        var httpResponse = await Client.PostAsJsonAsync("/vertex/query", new VertexQueryRequest(""));

        await AssertErrorAsync(httpResponse, HttpStatusCode.BadRequest, ErrorCodes.QueryRequired);
    }

    [Fact]
    public async Task SuggestDescription_UpstreamSucceeds_ReturnsSuggestion()
    {
        var project = await CreateProjectAsync(ProjectId.New(), title: "Described Project");
        factory.RespondWith(HttpStatusCode.OK, TextAnswer("  <div><p>New description</p></div>  "));

        var httpResponse = await AuthenticatedClient.PostAsJsonAsync(
            $"/vertex/projects/{project.Id}/description/suggest", new SuggestProjectDescriptionRequest());

        httpResponse.EnsureSuccessStatusCode();
        var response = GetJsonContent<SuggestionResponse>(await httpResponse.Content.ReadAsStringAsync());
        Assert.NotNull(response);
        Assert.Equal(project.Id.Value, response.ProjectId);
        Assert.Equal("Described Project", response.Title);
        Assert.Equal("<div><p>New description</p></div>", response.SuggestedDescription);
    }

    [Fact]
    public async Task SuggestDescription_NonExistingProject_ReturnsNotFound()
    {
        var httpResponse = await AuthenticatedClient.PostAsJsonAsync(
            $"/vertex/projects/{ProjectId.New()}/description/suggest", new SuggestProjectDescriptionRequest());

        await AssertErrorAsync(httpResponse, HttpStatusCode.NotFound, ErrorCodes.ProjectNotFound);
    }

    [Fact]
    public async Task SuggestDescription_UpstreamReturnsServerError_ReturnsUpstreamUnavailable()
    {
        var project = await CreateProjectAsync(ProjectId.New());
        factory.RespondWith(HttpStatusCode.ServiceUnavailable, "unavailable");

        var httpResponse = await AuthenticatedClient.PostAsJsonAsync(
            $"/vertex/projects/{project.Id}/description/suggest", new SuggestProjectDescriptionRequest());

        await AssertErrorAsync(httpResponse, HttpStatusCode.BadGateway, ErrorCodes.UpstreamUnavailable);
    }

    [Fact]
    public async Task SuggestDescription_UpstreamReturnsNoText_ReturnsUpstreamEmptyResponse()
    {
        var project = await CreateProjectAsync(ProjectId.New());
        factory.RespondWith(HttpStatusCode.OK, """{ "candidates": [] }""");

        var httpResponse = await AuthenticatedClient.PostAsJsonAsync(
            $"/vertex/projects/{project.Id}/description/suggest", new SuggestProjectDescriptionRequest());

        await AssertErrorAsync(httpResponse, HttpStatusCode.BadGateway, ErrorCodes.UpstreamEmptyResponse);
    }

    [Fact]
    public async Task Ingest_UpstreamSucceeds_ReturnsSummary()
    {
        factory.ChatbotClient.Reset();
        await CreateProjectAsync(ProjectId.New(), companyId: CompanyId.New(), tags: ["Ingested Tag"]);

        var httpResponse = await AuthenticatedClient.PostAsync("/vertex/ingest", null);

        httpResponse.EnsureSuccessStatusCode();
        var response = GetJsonContent<IngestResponse>(await httpResponse.Content.ReadAsStringAsync());
        Assert.NotNull(response);
        Assert.Equal(1, response.CompaniesIngested);
        Assert.Equal(1, response.TagsIngested);
        Assert.Equal(1, response.ProjectsIngested);
    }

    [Fact]
    public async Task Ingest_UpstreamUnavailable_ReturnsUpstreamUnavailable()
    {
        factory.ChatbotClient.Reset();
        factory.ChatbotClient
            .Setup(c => c.InitialiseCachesAsync(It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new RpcException(new Status(StatusCode.Unavailable, "Vertex is down")));

        var httpResponse = await AuthenticatedClient.PostAsync("/vertex/ingest", null);

        await AssertErrorAsync(httpResponse, HttpStatusCode.BadGateway, ErrorCodes.UpstreamUnavailable);
    }

    private static string TextAnswer(string text) =>
        $$"""{ "candidates": [ { "content": { "parts": [ { "text": {{JsonSerializer.Serialize(text)}} } ] }, "finishReason": "STOP" } ] }""";

    private static string GroundedAnswer(string text, ProjectId projectId) =>
        $$"""
        {
          "candidates": [
            {
              "content": { "parts": [ { "text": "{{text}}" } ] },
              "groundingMetadata": {
                "groundingChunks": [
                  {
                    "retrievedContext": {
                      "documentName": "projects/p/locations/global/collections/default_collection/dataStores/local_project_structured_v3/branches/0/documents/{{projectId}}",
                      "text": "Description: no title line here"
                    }
                  }
                ]
              },
              "finishReason": "STOP"
            }
          ]
        }
        """;

    private sealed record QueryResponse(string Answer, List<CitationResponse> Citations);

    private sealed record CitationResponse(Guid? Id, string? FeatureType, string? Title);

    private sealed record SuggestionResponse(Guid ProjectId, string Title, string ExistingDescription, string SuggestedDescription);

    private sealed record IngestResponse(int CompaniesIngested, int TagsIngested, int ProjectsIngested);
}
