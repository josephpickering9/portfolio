using Work_Experience_Search.Models;
using Work_Experience_Search.Repositories;
using Work_Experience_Search.Requests;
using Work_Experience_Search.Types;
using ProjectRepositoryEntity = Work_Experience_Search.Models.ProjectRepository;

namespace Work_Experience_Search.Services;

public sealed class ProjectRepositoryService(IProjectCodeRepository projectCodeRepository) : IProjectRepositoryService
{
    public async Task<Result<IReadOnlyList<ProjectRepositoryEntity>>> SyncProjectRepositoriesAsync(Project project, IReadOnlyList<CreateProjectRepository> repositories, CancellationToken cancellationToken = default)
    {
        var repositoryIds = repositories.Select(i => i.Id).ToList();
        var repositoriesToDelete = project.Repositories.Where(i => !repositoryIds.Contains(i.Id)).ToList();
        var repositoriesToCreate = repositories.Where(i => i.Id is null).ToList();
        var repositoriesToSave = project.Repositories.Where(i => repositories.Any(x => x.Id == i.Id)).ToList();

        foreach (var repository in repositoriesToSave)
        {
            ApplyDetails(repository, repositories.First(x => x.Id == repository.Id));
        }

        foreach (var repository in repositoriesToDelete)
        {
            await projectCodeRepository.RemoveAsync(repository, cancellationToken);
        }

        foreach (var request in repositoriesToCreate)
        {
            var repository = new ProjectRepositoryEntity { Project = project };
            ApplyDetails(repository, request);

            await projectCodeRepository.AddAsync(repository, cancellationToken);
            repositoriesToSave.Add(repository);
        }

        await projectCodeRepository.SaveChangesAsync(cancellationToken);

        return Result<IReadOnlyList<ProjectRepositoryEntity>>.Ok(repositoriesToSave);
    }

    private static void ApplyDetails(ProjectRepositoryEntity repository, CreateProjectRepository request)
    {
        repository.Title = request.Title;
        repository.Url = request.Url;
        repository.Order = request.Order;
    }
}
