using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Work_Experience_Search.Extensions;
using Work_Experience_Search.Models;
using Work_Experience_Search.Services;
using Work_Experience_Search.Types;

namespace Work_Experience_Search.Repositories;

public sealed class TagRepository(Database context, IMemoryCache cache, CacheInvalidator cacheInvalidator)
    : BaseRepository(cache), ITagRepository
{
    public async Task<IReadOnlyList<Tag>> GetByIdsAsync(IReadOnlyCollection<TagId> ids, CancellationToken cancellationToken = default)
    {
        return await context.Tag
            .Where(t => ids.Contains(t.Id))
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<Tag?> GetAsync(string slug, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"tag:slug:{slug}";
        if (TryGetCache(cacheKey, out Tag? cached)) return cached;

        var tag = await context.Tag
            .Include(t => t.Projects)
            .ThenInclude(p => p.Images)
            .Include(t => t.Projects)
            .ThenInclude(p => p.Tags)
            .AsNoTrackingWithIdentityResolution()
            .SingleOrDefaultAsync(t => t.Slug == slug, cancellationToken);

        SetCache(cacheKey, tag, cacheInvalidator.GetTagsChangeToken());
        return tag;
    }

    public async Task<Tag?> GetAsync(TagId id, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"tag:{id}";
        if (TryGetCache(cacheKey, out Tag? cached)) return cached;

        var tag = await context.Tag.AsNoTracking().SingleOrDefaultAsync(t => t.Id == id, cancellationToken);
        SetCache(cacheKey, tag, cacheInvalidator.GetTagsChangeToken());
        return tag;
    }

    public async Task<Tag?> GetTrackedAsync(TagId id, CancellationToken cancellationToken = default)
    {
        return await context.Tag.FindAsync([id], cancellationToken);
    }

    public Task<Tag?> GetByTitleTrackedAsync(string title, CancellationToken cancellationToken = default) =>
        context.Tag.FirstOrDefaultAsync(t => t.Title.ToLower() == title.ToLower(), cancellationToken);

    public async Task<IReadOnlyList<Tag>> SearchAsync(string? search, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"tags:{search ?? ""}";
        if (TryGetCache(cacheKey, out IReadOnlyList<Tag>? cached) && cached is not null) return cached;

        var tags = context.Tag.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
            tags = tags.Where(t => t.Title.ToLower().Contains(search.ToLower()));

        var result = await tags.ToListAsync(cancellationToken);
        SetCache(cacheKey, result, cacheInvalidator.GetTagsChangeToken());
        return result;
    }

    public Task<bool> ExistsAsync(string title, CancellationToken cancellationToken = default) =>
        context.Tag.AnyAsync(t => t.Title.ToLower() == title.ToLower(), cancellationToken);

    public Task<bool> ExistsAsync(string title, TagId excludeId, CancellationToken cancellationToken = default) =>
        context.Tag.AnyAsync(t => t.Id != excludeId && t.Title.ToLower() == title.ToLower(), cancellationToken);

    public Task<bool> SlugExistsAsync(string slug, TagId? excludeId, CancellationToken cancellationToken = default) =>
        context.Tag.AnyAsync(t => t.Slug == slug && t.Id != excludeId, cancellationToken);

    public async Task AddAsync(Tag tag, CancellationToken cancellationToken = default)
    {
        context.Tag.Add(tag);
        await context.SaveChangesAsync(cancellationToken);
        cacheInvalidator.InvalidateTags();
    }

    public async Task UpdateAsync(Tag tag, CancellationToken cancellationToken = default)
    {
        await context.SaveChangesAsync(cancellationToken);
        cacheInvalidator.InvalidateTags();
    }

    public async Task DeleteAsync(Tag tag, CancellationToken cancellationToken = default)
    {
        context.Tag.Remove(tag);
        await context.SaveChangesAsync(cancellationToken);
        cacheInvalidator.InvalidateTags();
    }
}
