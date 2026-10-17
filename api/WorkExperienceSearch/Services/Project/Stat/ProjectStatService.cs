using Work_Experience_Search.Requests;
using Work_Experience_Search.Models;
using Work_Experience_Search.Types;

using Work_Experience_Search.Repositories;

namespace Work_Experience_Search.Services;

public class ProjectStatService(IProjectRepository projectRepository, IProjectStatRepository projectStatRepository, ILogger<ProjectStatService> logger) : IProjectStatService
{
    public async Task<Result<IEnumerable<ProjectStat>>> GetProjectStatsAsync(ProjectId projectId)
    {
        var project = await projectRepository.GetAsync(projectId);
        if (project == null)
        {
            logger.LogWarning("Project {ProjectId} not found when listing stats.", projectId);
            return new NotFoundFailure<IEnumerable<ProjectStat>>("Project not found.");
        }

        return new Success<IEnumerable<ProjectStat>>(project.Stats);
    }

    public async Task<Result<ProjectStat>> GetProjectStatAsync(ProjectId projectId, ProjectStatId id)
    {
        var project = await projectRepository.GetAsync(projectId);
        if (project == null)
        {
            logger.LogWarning("Project {ProjectId} not found when fetching stat {ProjectStatId}.", projectId, id);
            return new NotFoundFailure<ProjectStat>("Project not found.");
        }

        var stat = project.Stats.SingleOrDefault(i => i.Id == id);
        if (stat == null)
        {
            logger.LogWarning("Stat {ProjectStatId} not found on project {ProjectId}.", id, projectId);
            return new NotFoundFailure<ProjectStat>("Stat not found.");
        }

        return new Success<ProjectStat>(stat);
    }

    public async Task<Result<List<ProjectStat>>> SyncProjectStatsAsync(ProjectId projectId, List<CreateProjectStat> stats)
    {
        var project = await projectRepository.GetAsync(projectId);
        if (project == null)
        {
            logger.LogWarning("Project {ProjectId} not found when syncing stats.", projectId);
            return new NotFoundFailure<List<ProjectStat>>("Project not found.");
        }

        return await SyncProjectStatsAsync(project, stats);
    }

    public async Task<Result<List<ProjectStat>>> SyncProjectStatsAsync(
        Project project,
        List<CreateProjectStat> stats
    )
    {
        var statIds = stats.Select(i => i.Id).ToList();
        var statsToDelete = project.Stats.Where(i => !statIds.Contains(i.Id)).ToList();
        var statsToCreate = stats.Where(i => i.Id == null).ToList();
        var statsToSave = project.Stats.Where(i => stats.Any(x => x.Id == i.Id)).ToList();

        foreach (var stat in statsToSave)
        {
            var request = stats.First(x => x.Id == stat.Id);
            stat.Label = request.Label;
            stat.Value = request.Value;
            stat.Order = request.Order;
        }

        foreach (var stat in statsToDelete)
        {
            await projectStatRepository.RemoveAsync(stat);
        }

        foreach (var stat in statsToCreate)
        {
            var projectStat = new ProjectStat
            {
                Label = stat.Label,
                Value = stat.Value,
                Order = stat.Order,
                Project = project
            };

            await projectStatRepository.AddAsync(projectStat);

            statsToSave.Add(projectStat);
        }

        await projectStatRepository.SaveChangesAsync();

        return new Success<List<ProjectStat>>(statsToSave);
    }
}
