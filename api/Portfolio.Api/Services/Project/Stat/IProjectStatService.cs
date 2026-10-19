using Portfolio.Api.Models;
using Portfolio.Api.Requests;
using Portfolio.Api.Types;

namespace Portfolio.Api.Services;

public interface IProjectStatService
{
    Task<Result<IReadOnlyList<ProjectStat>>> SyncProjectStatsAsync(Project project, IReadOnlyList<CreateProjectStat> stats, CancellationToken cancellationToken = default);
}
