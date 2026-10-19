using Portfolio.Api.Extensions;
using Portfolio.Api.Models;
using Portfolio.Api.Repositories;
using Portfolio.Api.Requests;
using Portfolio.Api.Types;

namespace Portfolio.Api.Services;

public sealed class TagService(ITagRepository repository, ILogger<TagService> logger) : ITagService
{
    private const string SlugFallback = "tag";

    public async Task<Result<IReadOnlyList<Tag>>> GetTagsAsync(string? search, CancellationToken cancellationToken = default)
    {
        var tags = await repository.SearchAsync(search, cancellationToken);
        return tags.ToOk();
    }

    public async Task<Result<Tag>> GetTagAsync(TagId id, CancellationToken cancellationToken = default) =>
        (await repository.GetAsync(id, cancellationToken)).ToResultOrFail(ErrorCodes.TagNotFound);

    public async Task<Result<Tag>> GetTagBySlugAsync(string slug, CancellationToken cancellationToken = default) =>
        (await repository.GetAsync(slug, cancellationToken)).ToResultOrFail(ErrorCodes.TagNotFound);

    public async Task<Result<Tag>> CreateTagAsync(CreateTag createTag, CancellationToken cancellationToken = default)
    {
        if (await repository.ExistsAsync(createTag.Title, cancellationToken))
        {
            logger.LogWarning("Tag creation conflict: a tag titled {Title} already exists.", createTag.Title);
            return ErrorCodes.TagAlreadyExists.ToFail();
        }

        var tag = new Tag();
        await ApplyDetailsAsync(tag, createTag, cancellationToken);

        await repository.AddAsync(tag, cancellationToken);

        return tag.ToOk();
    }

    public async Task<Result<IReadOnlyList<Tag>>> SyncTagsAsync(IReadOnlyList<string> tags, CancellationToken cancellationToken = default)
    {
        var syncedTags = new List<Tag>();

        foreach (var title in tags)
        {
            var existingTag = await repository.GetByTitleTrackedAsync(title, cancellationToken);
            if (existingTag is not null)
            {
                syncedTags.Add(existingTag);
                continue;
            }

            var newTag = await CreateDefaultTagAsync(title, cancellationToken);
            await repository.AddAsync(newTag, cancellationToken);
            syncedTags.Add(newTag);
        }

        return Result<IReadOnlyList<Tag>>.Ok(syncedTags);
    }

    public async Task<Result<Tag>> UpdateTagAsync(TagId id, CreateTag updateTag, CancellationToken cancellationToken = default)
    {
        var tag = await repository.GetTrackedAsync(id, cancellationToken);
        if (tag is null)
        {
            logger.LogWarning("Tag {TagId} not found for update.", id);
            return ErrorCodes.TagNotFound.ToFail();
        }

        if (await repository.ExistsAsync(updateTag.Title, id, cancellationToken))
        {
            logger.LogWarning("Tag {TagId} update conflict: a tag titled {Title} already exists.", id, updateTag.Title);
            return ErrorCodes.TagAlreadyExists.ToFail();
        }

        await ApplyDetailsAsync(tag, updateTag, cancellationToken);

        await repository.UpdateAsync(tag, cancellationToken);

        return tag.ToOk();
    }

    public async Task<Result> DeleteTagAsync(TagId id, CancellationToken cancellationToken = default)
    {
        var tag = await repository.GetTrackedAsync(id, cancellationToken);
        if (tag is null)
        {
            logger.LogWarning("Tag {TagId} not found for deletion.", id);
            return ErrorCodes.TagNotFound.ToFail();
        }

        await repository.DeleteAsync(tag, cancellationToken);

        return Result.Ok();
    }

    private async Task ApplyDetailsAsync(Tag tag, CreateTag request, CancellationToken cancellationToken)
    {
        if (tag.Title != request.Title)
            tag.Slug = await ResolveSlugAsync(request.Title, tag.Id, cancellationToken);

        tag.Title = request.Title;
        tag.Type = request.Type;
        tag.Icon = request.Icon;
        tag.CustomColour = request.CustomColour;
    }

    private async Task<Tag> CreateDefaultTagAsync(string title, CancellationToken cancellationToken) =>
        new()
        {
            Title = title,
            Type = TagType.Default,
            Icon = string.Empty,
            CustomColour = null,
            Slug = await ResolveSlugAsync(title, null, cancellationToken)
        };

    private Task<string> ResolveSlugAsync(string title, TagId? excludeId, CancellationToken cancellationToken) =>
        title.ToUniqueSlugAsync(SlugFallback, (slug, ct) => repository.SlugExistsAsync(slug, excludeId, ct), cancellationToken);
}
