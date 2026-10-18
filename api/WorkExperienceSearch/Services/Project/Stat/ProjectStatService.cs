using Work_Experience_Search.Models;
using Work_Experience_Search.Repositories;
using Work_Experience_Search.Requests;
using Work_Experience_Search.Types;

namespace Work_Experience_Search.Services;

public sealed class ProjectStatService(
    IProjectRepository projectRepository,
    IProjectStatRepository projectStatRepository,
    ILogger<ProjectStatService> logger) : IProjectStatService
{
    public async Task<Result<IReadOnlyList<ProjectStat>>> GetProjectStatsAsync(ProjectId projectId, CancellationToken cancellationToken = default)
    {
        var project = await projectRepository.GetAsync(projectId, cancellationToken);
        if (project is null)
        {
            logger.LogWarning("Project {ProjectId} not found when listing stats.", projectId);
            return ErrorCodes.ProjectNotFound.ToFail();
        }

        return Result<IReadOnlyList<ProjectStat>>.Ok(project.Stats);
    }

    public async Task<Result<ProjectStat>> GetProjectStatAsync(ProjectId projectId, ProjectStatId id, CancellationToken cancellationToken = default)
    {
        var project = await projectRepository.GetAsync(projectId, cancellationToken);
        if (project is null)
        {
            logger.LogWarning("Project {ProjectId} not found when fetching stat {ProjectStatId}.", projectId, id);
            return ErrorCodes.ProjectNotFound.ToFail();
        }

        var stat = project.Stats.SingleOrDefault(i => i.Id == id);
        if (stat is null)
        {
            logger.LogWarning("Stat {ProjectStatId} not found on project {ProjectId}.", id, projectId);
            return ErrorCodes.ProjectStatNotFound.ToFail();
        }

        return stat.ToOk();
    }

    public async Task<Result<IReadOnlyList<ProjectStat>>> SyncProjectStatsAsync(ProjectId projectId, IReadOnlyList<CreateProjectStat> stats, CancellationToken cancellationToken = default)
    {
        var project = await projectRepository.GetTrackedAsync(projectId, cancellationToken);
        if (project is null)
        {
            logger.LogWarning("Project {ProjectId} not found when syncing stats.", projectId);
            return ErrorCodes.ProjectNotFound.ToFail();
        }

        return await SyncProjectStatsAsync(project, stats, cancellationToken);
    }

    public async Task<Result<IReadOnlyList<ProjectStat>>> SyncProjectStatsAsync(Project project, IReadOnlyList<CreateProjectStat> stats, CancellationToken cancellationToken = default)
    {
        var statIds = stats.Select(i => i.Id).ToList();
        var statsToDelete = project.Stats.Where(i => !statIds.Contains(i.Id)).ToList();
        var statsToCreate = stats.Where(i => i.Id is null).ToList();
        var statsToSave = project.Stats.Where(i => stats.Any(x => x.Id == i.Id)).ToList();

        foreach (var stat in statsToSave)
        {
            ApplyDetails(stat, stats.First(x => x.Id == stat.Id));
        }

        foreach (var stat in statsToDelete)
        {
            await projectStatRepository.RemoveAsync(stat, cancellationToken);
        }

        foreach (var request in statsToCreate)
        {
            var stat = new ProjectStat { Project = project };
            ApplyDetails(stat, request);

            await projectStatRepository.AddAsync(stat, cancellationToken);
            statsToSave.Add(stat);
        }

        await projectStatRepository.SaveChangesAsync(cancellationToken);

        return Result<IReadOnlyList<ProjectStat>>.Ok(statsToSave);
    }

    private static void ApplyDetails(ProjectStat stat, CreateProjectStat request)
    {
        stat.Label = request.Label;
        stat.Value = request.Value;
        stat.Order = request.Order;
    }
}
