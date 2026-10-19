using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Work_Experience_Search.Extensions;
using Work_Experience_Search.Models;
using Work_Experience_Search.Services;
using Work_Experience_Search.Types;

namespace Work_Experience_Search.Repositories;

public sealed class ProjectRepository(Database context, IMemoryCache cache, CacheInvalidator cacheInvalidator)
    : BaseRepository(cache), IProjectRepository
{
    private const int RelatedProjectsLimit = 3;

    public async Task<IReadOnlyList<Project>> SearchAsync(string? search, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"projects:{search ?? ""}";
        if (TryGetCache(cacheKey, out IReadOnlyList<Project>? cached) && cached is not null) return cached;

        var projects = WithIncludes(context.Project).AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.ToLower();
            projects = projects.Where(p => p.Title.ToLower().Contains(term) || p.ShortDescription.ToLower().Contains(term));
        }

        var result = await projects.OrderByDescending(p => p.StartDate).ThenByDescending(p => p.EndDate).ToListAsync(cancellationToken);
        SetCache(cacheKey, result, cacheInvalidator.GetProjectsChangeToken());
        return result;
    }

    public async Task<IReadOnlyList<Project>> GetByIdsAsync(IReadOnlyCollection<ProjectId> ids, CancellationToken cancellationToken = default)
    {
        return await context.Project
            .Include(p => p.Tags)
            .Include(p => p.Images)
            .Include(p => p.Repositories)
            .Include(p => p.Stats)
            .Where(p => ids.Contains(p.Id))
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<Project?> GetAsync(ProjectId id, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"project:{id}";
        if (TryGetCache(cacheKey, out Project? cached)) return cached;

        var project = await WithIncludes(context.Project)
            .Include(p => p.Company)
            .AsNoTracking()
            .SingleOrDefaultAsync(p => p.Id == id, cancellationToken);

        SetCache(cacheKey, project, cacheInvalidator.GetProjectsChangeToken());
        return project;
    }

    public async Task<Project?> GetAsync(string slug, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"project:slug:{slug}";
        if (TryGetCache(cacheKey, out Project? cached)) return cached;

        var project = await WithIncludes(context.Project)
            .AsNoTracking()
            .SingleOrDefaultAsync(p => p.Slug == slug, cancellationToken);

        SetCache(cacheKey, project, cacheInvalidator.GetProjectsChangeToken());
        return project;
    }

    public async Task<Project?> GetTrackedAsync(ProjectId id, CancellationToken cancellationToken = default)
    {
        return await WithIncludes(context.Project)
            .Include(p => p.Company)
            .SingleOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Project>> GetRelatedAsync(ProjectId projectId, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"project:related:{projectId}";
        if (TryGetCache(cacheKey, out IReadOnlyList<Project>? cached) && cached is not null) return cached;

        var projectTags = context.Project
            .Where(pt => pt.Id == projectId)
            .SelectMany(pt => pt.Tags.Select(t => t.Id));

        if (!await projectTags.AnyAsync(cancellationToken))
            return [];

        var related = await context.Project
            .Where(p => p.Id != projectId && p.Tags.Any(t => projectTags.Contains(t.Id)))
            .Select(p => new
            {
                Project = p,
                SharedTagsCount = p.Tags.Count(t => projectTags.Contains(t.Id))
            })
            .OrderByDescending(x => x.SharedTagsCount)
            .Take(RelatedProjectsLimit)
            .Select(x => x.Project)
            .Include(p => p.Tags)
            .Include(p => p.Images)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        SetCache(cacheKey, related, cacheInvalidator.GetProjectsChangeToken());
        return related;
    }

    public Task<bool> ExistsAsync(string title, CancellationToken cancellationToken = default) =>
        context.Project.AnyAsync(p => p.Title.ToLower() == title.ToLower(), cancellationToken);

    public Task<bool> ExistsAsync(string title, ProjectId excludeId, CancellationToken cancellationToken = default) =>
        context.Project.AnyAsync(p => p.Id != excludeId && p.Title.ToLower() == title.ToLower(), cancellationToken);

    public Task<bool> SlugExistsAsync(string slug, ProjectId? excludeId, CancellationToken cancellationToken = default) =>
        context.Project.AnyAsync(p => p.Slug == slug && p.Id != excludeId, cancellationToken);

    public async Task AddAsync(Project project, CancellationToken cancellationToken = default)
    {
        context.Project.Add(project);
        await context.SaveChangesAsync(cancellationToken);
        cacheInvalidator.InvalidateProjects();
    }

    public async Task UpdateAsync(Project project, CancellationToken cancellationToken = default)
    {
        context.Entry(project).State = EntityState.Modified;
        await context.SaveChangesAsync(cancellationToken);
        cacheInvalidator.InvalidateProjects();
    }

    public async Task DeleteAsync(Project project, CancellationToken cancellationToken = default)
    {
        context.Project.Remove(project);
        await context.SaveChangesAsync(cancellationToken);
        cacheInvalidator.InvalidateProjects();
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await context.SaveChangesAsync(cancellationToken);
        cacheInvalidator.InvalidateProjects();
    }

    private static IQueryable<Project> WithIncludes(IQueryable<Project> query) =>
        query
            .Include(p => p.Tags)
            .Include(p => p.Images.OrderBy(i => i.Type).ThenBy(i => i.Order ?? 0))
            .Include(p => p.Repositories.OrderBy(i => i.Order ?? 0))
            .Include(p => p.Stats.OrderBy(i => i.Order ?? 0));
}
