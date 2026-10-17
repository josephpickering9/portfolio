using Work_Experience_Search.Models;

namespace Work_Experience_Search.Repositories;

public interface IProjectStatRepository
{
    Task AddAsync(ProjectStat stat, CancellationToken cancellationToken = default);
    Task RemoveAsync(ProjectStat stat, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
