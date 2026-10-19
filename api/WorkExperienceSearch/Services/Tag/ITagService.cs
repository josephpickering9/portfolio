using Work_Experience_Search.Models;
using Work_Experience_Search.Requests;
using Work_Experience_Search.Types;

namespace Work_Experience_Search.Services;

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
