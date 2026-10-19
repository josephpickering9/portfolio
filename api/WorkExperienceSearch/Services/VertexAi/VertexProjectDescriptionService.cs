using Work_Experience_Search.Models;
using Work_Experience_Search.Repositories;
using Work_Experience_Search.Requests;
using Work_Experience_Search.Types;

namespace Work_Experience_Search.Services.VertexAi;

public interface IVertexProjectDescriptionService
{
    Task<Result<ProjectDescriptionSuggestionResponse>> SuggestDescriptionAsync(ProjectId projectId, SuggestProjectDescriptionRequest request, CancellationToken cancellationToken = default);
}

public sealed class VertexProjectDescriptionService(
    IVertexGenerateContentClient generateContentClient,
    IProjectRepository projectRepository,
    ILogger<VertexProjectDescriptionService> logger) : IVertexProjectDescriptionService
{
    public async Task<Result<ProjectDescriptionSuggestionResponse>> SuggestDescriptionAsync(ProjectId projectId, SuggestProjectDescriptionRequest request, CancellationToken cancellationToken = default)
    {
        var project = await projectRepository.GetAsync(projectId, cancellationToken);
        if (project is null)
        {
            logger.LogWarning("Project {ProjectId} not found for description suggestion.", projectId);
            return ErrorCodes.ProjectNotFound.ToFail();
        }

        var generateRequest = new GenerateContentRequest(
            GenerateContentMessage.FromSystem(VertexProjectDescriptionPrompt.SystemPrompt),
            [GenerateContentMessage.FromUser(VertexProjectDescriptionPrompt.Build(project, request))]);

        var response = await generateContentClient.GenerateContentAsync(generateRequest, cancellationToken);
        if (!response.IsSuccess) return response.Error!.ToFail();

        var answer = ExtractAnswer(response.Value!);
        if (string.IsNullOrWhiteSpace(answer))
        {
            logger.LogError("Vertex description generation returned no text for project {ProjectId}.", projectId);
            return ErrorCodes.UpstreamEmptyResponse.ToFail();
        }

        return ToResponse(project, answer.Trim()).ToOk();
    }

    private static string? ExtractAnswer(GenerateContentResponse response)
    {
        var textParts = response.Candidates.FirstOrDefault()?.Content?.Parts
            .Select(part => part.Text)
            .Where(text => !string.IsNullOrWhiteSpace(text))
            .ToList();

        return textParts is { Count: > 0 } ? string.Join(string.Empty, textParts) : null;
    }

    private static ProjectDescriptionSuggestionResponse ToResponse(Project project, string suggestedDescription) =>
        new(
            ProjectId: project.Id,
            Title: project.Title,
            ExistingDescription: project.Description,
            SuggestedDescription: suggestedDescription);
}

public sealed record ProjectDescriptionSuggestionResponse(ProjectId ProjectId, string Title, string ExistingDescription, string SuggestedDescription);
