using Portfolio.Api.Models;
using Portfolio.Api.Requests;
using Portfolio.Api.Types;

namespace Portfolio.Api.Services;

public interface ITagService
{
    Task<Result<IReadOnlyList<Tag>>> GetTagsAsync(string? search, CancellationToken cancellationToken = default);
    Task<Result<Tag>> GetTagAsync(TagId id, CancellationToken cancellationToken = default);
    Task<Result<Tag>> GetTagBySlugAsync(string slug, CancellationToken cancellationToken = default);
    Task<Result<Tag>> CreateTagAsync(CreateTag createTag, CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyList<Tag>>> SyncTagsAsync(IReadOnlyList<string> tags, CancellationToken cancellationToken = default);
    Task<Result<Tag>> UpdateTagAsync(TagId id, CreateTag updateTag, CancellationToken cancellationToken = default);
    Task<Result> DeleteTagAsync(TagId id, CancellationToken cancellationToken = default);
}
