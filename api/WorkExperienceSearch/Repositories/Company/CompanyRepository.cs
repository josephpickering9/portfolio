using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Work_Experience_Search.Extensions;
using Work_Experience_Search.Models;
using Work_Experience_Search.Services;
using Work_Experience_Search.Types;

namespace Work_Experience_Search.Repositories;

public sealed class CompanyRepository(Database context, IMemoryCache cache, CacheInvalidator cacheInvalidator)
    : BaseRepository(cache), ICompanyRepository
{
    public async Task<IReadOnlyList<Company>> GetByIdsAsync(IReadOnlyCollection<CompanyId> ids, CancellationToken cancellationToken = default)
    {
        return await context.Company
            .Where(c => ids.Contains(c.Id))
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<Company?> GetAsync(string slug, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"company:slug:{slug}";
        if (TryGetCache(cacheKey, out Company? cached)) return cached;

        var company = await context.Company.AsNoTracking().SingleOrDefaultAsync(c => c.Slug == slug, cancellationToken);
        SetCache(cacheKey, company, cacheInvalidator.GetCompaniesChangeToken());
        return company;
    }

    public async Task<Company?> GetAsync(CompanyId id, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"company:{id}";
        if (TryGetCache(cacheKey, out Company? cached)) return cached;

        var company = await context.Company.AsNoTracking().SingleOrDefaultAsync(c => c.Id == id, cancellationToken);
        SetCache(cacheKey, company, cacheInvalidator.GetCompaniesChangeToken());
        return company;
    }

    public async Task<Company?> GetTrackedAsync(CompanyId id, CancellationToken cancellationToken = default)
    {
        return await context.Company.FindAsync([id], cancellationToken);
    }

    public async Task<IReadOnlyList<Company>> SearchAsync(string? search, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"companies:{search ?? ""}";
        if (TryGetCache(cacheKey, out IReadOnlyList<Company>? cached) && cached is not null) return cached;

        var companies = context.Company.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var normalizedSearch = search.ToLowerInvariant();
            companies = context.Database.SupportsILike()
                ? companies.Where(c => EF.Functions.ILike(c.Name, $"%{search}%"))
                : companies.Where(c => c.Name.ToLower().Contains(normalizedSearch));
        }

        var result = await companies.ToListAsync(cancellationToken);
        SetCache(cacheKey, result, cacheInvalidator.GetCompaniesChangeToken());
        return result;
    }

    public async Task<bool> ExistsAsync(string name, CancellationToken cancellationToken = default)
    {
        return context.Database.SupportsILike()
            ? await context.Company.AnyAsync(c => EF.Functions.ILike(c.Name, name), cancellationToken)
            : await context.Company.AnyAsync(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase), cancellationToken);
    }

    public async Task<bool> ExistsAsync(string name, CompanyId excludeId, CancellationToken cancellationToken = default)
    {
        return context.Database.SupportsILike()
            ? await context.Company.AnyAsync(c => c.Id != excludeId && EF.Functions.ILike(c.Name, name), cancellationToken)
            : await context.Company.AnyAsync(c => c.Id != excludeId && c.Name.Equals(name, StringComparison.OrdinalIgnoreCase), cancellationToken);
    }

    public async Task AddAsync(Company company, CancellationToken cancellationToken = default)
    {
        context.Company.Add(company);
        await context.SaveChangesAsync(cancellationToken);
        cacheInvalidator.InvalidateCompanies();
    }

    public async Task UpdateAsync(Company company, CancellationToken cancellationToken = default)
    {
        await context.SaveChangesAsync(cancellationToken);
        cacheInvalidator.InvalidateCompanies();
    }

    public async Task DeleteAsync(Company company, CancellationToken cancellationToken = default)
    {
        context.Company.Remove(company);
        await context.SaveChangesAsync(cancellationToken);
        cacheInvalidator.InvalidateCompanies();
    }
}
