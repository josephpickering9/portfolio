using Work_Experience_Search.Extensions;
using Work_Experience_Search.Models;
using Work_Experience_Search.Repositories;
using Work_Experience_Search.Requests;
using Work_Experience_Search.Types;

namespace Work_Experience_Search.Services;

public sealed class TagService(ITagRepository repository, ILogger<TagService> logger) : ITagService
{
    public async Task<Result<IReadOnlyList<Tag>>> GetTagsAsync(string? search, CancellationToken cancellationToken = default)
    {
        var tags = await repository.SearchAsync(search, cancellationToken);
        return tags.ToOk();
    }

    public async Task<Result<Tag>> GetTagAsync(TagId id, CancellationToken cancellationToken = default)
    {
        var tag = await repository.GetAsync(id, cancellationToken);
        if (tag is null)
        {
            logger.LogWarning("Tag {TagId} not found.", id);
            return ErrorCodes.TagNotFound.ToFail();
        }

        return tag.ToOk();
    }

    public async Task<Result<Tag>> GetTagBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        var tag = await repository.GetAsync(slug, cancellationToken);
        if (tag is null)
        {
            logger.LogWarning("Tag with slug {Slug} not found.", slug);
            return ErrorCodes.TagNotFound.ToFail();
        }

        return tag.ToOk();
    }

    public async Task<Result<Tag>> CreateTagAsync(CreateTag createTag, CancellationToken cancellationToken = default)
    {
        if (await repository.ExistsAsync(createTag.Title, cancellationToken))
        {
            logger.LogWarning("Tag creation conflict: a tag titled {Title} already exists.", createTag.Title);
            return ErrorCodes.TagAlreadyExists.ToFail();
        }

        var tag = new Tag();
        ApplyDetails(tag, createTag);

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

            var newTag = CreateDefaultTag(title);
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

        ApplyDetails(tag, updateTag);

        await repository.UpdateAsync(tag, cancellationToken);

        return tag.ToOk();
    }

    public async Task<Result<Tag>> DeleteTagAsync(TagId id, CancellationToken cancellationToken = default)
    {
        var tag = await repository.GetTrackedAsync(id, cancellationToken);
        if (tag is null)
        {
            logger.LogWarning("Tag {TagId} not found for deletion.", id);
            return ErrorCodes.TagNotFound.ToFail();
        }

        await repository.DeleteAsync(tag, cancellationToken);

        return tag.ToOk();
    }

    private static void ApplyDetails(Tag tag, CreateTag request)
    {
        tag.Title = request.Title;
        tag.Type = request.Type;
        tag.Icon = request.Icon;
        tag.CustomColour = request.CustomColour;
        tag.Slug = request.Title.ToSlug();
    }

    private static Tag CreateDefaultTag(string title) =>
        new()
        {
            Title = title,
            Type = TagType.Default,
            Icon = string.Empty,
            CustomColour = null,
            Slug = title.ToSlug()
        };
}
