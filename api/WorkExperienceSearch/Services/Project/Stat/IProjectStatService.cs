using Work_Experience_Search.Requests;
using Work_Experience_Search.Models;
using Work_Experience_Search.Types;

namespace Work_Experience_Search.Services;

public interface IProjectStatService
{
    Task<Result<IEnumerable<ProjectStat>>> GetProjectStatsAsync(ProjectId projectId);
    Task<Result<ProjectStat>> GetProjectStatAsync(ProjectId projectId, ProjectStatId id);
    Task<Result<List<ProjectStat>>> SyncProjectStatsAsync(ProjectId projectId, List<CreateProjectStat> stats);
    Task<Result<List<ProjectStat>>> SyncProjectStatsAsync(Project project, List<CreateProjectStat> stats);
}
