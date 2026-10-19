using Portfolio.Api.Models;
using Portfolio.Api.Types;

namespace Portfolio.Api.Repositories;

public interface ITagRepository
{
    Task<IReadOnlyList<Tag>> GetByIdsAsync(IReadOnlyCollection<TagId> ids, CancellationToken cancellationToken = default);
    Task<Tag?> GetAsync(string slug, CancellationToken cancellationToken = default);
    Task<Tag?> GetAsync(TagId id, CancellationToken cancellationToken = default);
    Task<Tag?> GetTrackedAsync(TagId id, CancellationToken cancellationToken = default);
    Task<Tag?> GetByTitleTrackedAsync(string title, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Tag>> SearchAsync(string? search, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(string title, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(string title, TagId excludeId, CancellationToken cancellationToken = default);
    Task<bool> SlugExistsAsync(string slug, TagId? excludeId, CancellationToken cancellationToken = default);
    Task AddAsync(Tag tag, CancellationToken cancellationToken = default);
    Task UpdateAsync(Tag tag, CancellationToken cancellationToken = default);
    Task DeleteAsync(Tag tag, CancellationToken cancellationToken = default);
}
