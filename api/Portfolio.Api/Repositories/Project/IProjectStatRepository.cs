using Portfolio.Api.Models;

namespace Portfolio.Api.Repositories;

public interface IProjectStatRepository
{
    Task AddAsync(ProjectStat stat, CancellationToken cancellationToken = default);
    Task RemoveAsync(ProjectStat stat, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
