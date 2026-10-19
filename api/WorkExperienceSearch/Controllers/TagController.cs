using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Work_Experience_Search.Extensions;
using Work_Experience_Search.Models;
using Work_Experience_Search.Requests;
using Work_Experience_Search.Services;
using Work_Experience_Search.Types;

namespace Work_Experience_Search.Controllers;

[ApiController]
[Route("[controller]")]
public sealed class TagController(ITagService tagService) : ControllerBase
{
    [HttpGet]
    public Task<ActionResult<IReadOnlyList<Tag>>> GetTags([FromQuery] SearchQuery query, CancellationToken cancellationToken) =>
        tagService.GetTagsAsync(query.Search, cancellationToken).ToResponse();

    [HttpGet("{id:guid}")]
    public Task<ActionResult<Tag>> GetTagById(TagId id, CancellationToken cancellationToken) =>
        tagService.GetTagAsync(id, cancellationToken).ToResponse();

    [HttpGet("{slug}")]
    public Task<ActionResult<Tag>> GetTagBySlug(string slug, CancellationToken cancellationToken) =>
        tagService.GetTagBySlugAsync(slug, cancellationToken).ToResponse();

    [HttpPost]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public Task<ActionResult<Tag>> PostTag([FromBody] CreateTag createTag, CancellationToken cancellationToken) =>
        tagService.CreateTagAsync(createTag, cancellationToken)
            .ToCreatedResponse(nameof(GetTagById), tag => new { id = tag.Id });

    [HttpPut("{id:guid}")]
    [Authorize]
    public Task<ActionResult<Tag>> PutTag(TagId id, [FromBody] CreateTag createTag, CancellationToken cancellationToken) =>
        tagService.UpdateTagAsync(id, createTag, cancellationToken).ToResponse();

    [HttpDelete("{id:guid}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public Task<IActionResult> DeleteTag(TagId id, CancellationToken cancellationToken) =>
        tagService.DeleteTagAsync(id, cancellationToken).ToNoContentOrError();
}
