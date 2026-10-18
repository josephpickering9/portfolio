using Work_Experience_Search.Extensions;
using Work_Experience_Search.Models;
using Work_Experience_Search.Repositories;
using Work_Experience_Search.Requests;
using Work_Experience_Search.Types;

namespace Work_Experience_Search.Services;

public sealed class ProjectService(
    IProjectRepository projectRepository,
    IProjectImageService projectImageService,
    IProjectRepositoryService projectRepositoryService,
    IProjectStatService projectStatService,
    ITagService tagService,
    ILogger<ProjectService> logger
) : IProjectService
{
    public async Task<Result<IReadOnlyList<Project>>> GetProjectsAsync(string? search, CancellationToken cancellationToken = default)
    {
        var projects = await projectRepository.SearchAsync(search, cancellationToken);
        return projects.ToOk();
    }

    public async Task<Result<Project>> GetProjectAsync(ProjectId id, CancellationToken cancellationToken = default)
    {
        var project = await projectRepository.GetAsync(id, cancellationToken);
        if (project is null)
        {
            logger.LogWarning("Project {ProjectId} not found.", id);
            return ErrorCodes.ProjectNotFound.ToFail();
        }

        return project.ToOk();
    }

    public async Task<Result<Project>> GetProjectBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        var project = await projectRepository.GetAsync(slug, cancellationToken);
        if (project is null)
        {
            logger.LogWarning("Project with slug {Slug} not found.", slug);
            return ErrorCodes.ProjectNotFound.ToFail();
        }

        return project.ToOk();
    }

    public async Task<Result<IReadOnlyList<Project>>> GetRelatedProjectsAsync(ProjectId projectId, CancellationToken cancellationToken = default)
    {
        var projects = await projectRepository.GetRelatedAsync(projectId, cancellationToken);
        return projects.ToOk();
    }

    public async Task<Result<Project>> CreateProjectAsync(CreateProject createProject, CancellationToken cancellationToken = default)
    {
        if (await projectRepository.ExistsAsync(createProject.Title, cancellationToken))
        {
            logger.LogWarning("Project creation conflict: a project titled {Title} already exists.", createProject.Title);
            return ErrorCodes.ProjectAlreadyExists.ToFail();
        }

        var project = new Project();
        ApplyDetails(project, createProject);

        await projectRepository.AddAsync(project, cancellationToken);

        var relationsResult = await SyncProjectRelationsAsync(project, createProject, cancellationToken);
        if (!relationsResult.IsSuccess)
        {
            logger.LogWarning("Project {ProjectId} relations sync failed during creation: {ErrorCode}", project.Id, relationsResult.Error);
            await projectRepository.DeleteAsync(project, cancellationToken);
            return relationsResult;
        }

        await projectRepository.SaveChangesAsync(cancellationToken);

        return project.ToOk();
    }

    public async Task<Result<Project>> UpdateProjectAsync(ProjectId id, CreateProject createProject, CancellationToken cancellationToken = default)
    {
        var project = await projectRepository.GetTrackedAsync(id, cancellationToken);
        if (project is null)
        {
            logger.LogWarning("Project {ProjectId} not found for update.", id);
            return ErrorCodes.ProjectNotFound.ToFail();
        }

        if (await projectRepository.ExistsAsync(createProject.Title, id, cancellationToken))
        {
            logger.LogWarning("Project {ProjectId} update conflict: a project titled {Title} already exists.", id, createProject.Title);
            return ErrorCodes.ProjectAlreadyExists.ToFail();
        }

        ApplyDetails(project, createProject);

        var relationsResult = await SyncProjectRelationsAsync(project, createProject, cancellationToken);
        if (!relationsResult.IsSuccess)
        {
            logger.LogWarning("Project {ProjectId} relations sync failed during update: {ErrorCode}", id, relationsResult.Error);
            return relationsResult;
        }

        await projectRepository.UpdateAsync(project, cancellationToken);

        return project.ToOk();
    }

    public async Task<Result<Project>> DeleteProjectAsync(ProjectId id, CancellationToken cancellationToken = default)
    {
        var project = await projectRepository.GetTrackedAsync(id, cancellationToken);
        if (project is null)
        {
            logger.LogWarning("Project {ProjectId} not found for deletion.", id);
            return ErrorCodes.ProjectNotFound.ToFail();
        }

        await projectRepository.DeleteAsync(project, cancellationToken);

        return project.ToOk();
    }

    private static void ApplyDetails(Project project, CreateProject request)
    {
        project.Title = request.Title;
        project.ShortDescription = request.ShortDescription;
        project.Description = request.Description;
        project.ProblemStatement = request.ProblemStatement;
        project.KeyDecision = request.KeyDecision;
        project.Outcome = request.Outcome;
        project.CompanyId = request.CompanyId;
        project.StartDate = request.StartDate;
        project.EndDate = request.EndDate;
        project.Website = request.Website;
        project.ShowMockup = request.ShowMockup;
        project.Slug = request.Title.ToSlug();
    }

    private async Task<Result<Project>> SyncProjectRelationsAsync(Project project, CreateProject createProject, CancellationToken cancellationToken)
    {
        var tags = await tagService.SyncTagsAsync(createProject.Tags, cancellationToken);
        if (!tags.IsSuccess) return tags.Error!.ToFail();
        project.Tags = [.. tags.Data!];

        var images = await projectImageService.SyncProjectImagesAsync(project, createProject.Images, cancellationToken);
        if (!images.IsSuccess) return images.Error!.ToFail();
        project.Images = [.. images.Data!];

        var repositories = await projectRepositoryService.SyncProjectRepositoriesAsync(project, createProject.Repositories, cancellationToken);
        if (!repositories.IsSuccess) return repositories.Error!.ToFail();
        project.Repositories = [.. repositories.Data!];

        var stats = await projectStatService.SyncProjectStatsAsync(project, createProject.Stats, cancellationToken);
        if (!stats.IsSuccess) return stats.Error!.ToFail();
        project.Stats = [.. stats.Data!];

        return project.ToOk();
    }
}
