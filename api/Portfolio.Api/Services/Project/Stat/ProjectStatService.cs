using Portfolio.Api.Models;
using Portfolio.Api.Repositories;
using Portfolio.Api.Requests;
using Portfolio.Api.Types;

namespace Portfolio.Api.Services;

public sealed class ProjectStatService(IProjectStatRepository projectStatRepository) : IProjectStatService
{
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
