using Work_Experience_Search.Models;
using Work_Experience_Search.Requests;
using Work_Experience_Search.Types;

namespace Work_Experience_Search.Services;

public interface IProjectStatService
{
    Task<Result<IReadOnlyList<ProjectStat>>> SyncProjectStatsAsync(Project project, IReadOnlyList<CreateProjectStat> stats, CancellationToken cancellationToken = default);
}
