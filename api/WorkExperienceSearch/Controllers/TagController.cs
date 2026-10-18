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
    public async Task<ActionResult<IReadOnlyList<Tag>>> GetTags(string? search, CancellationToken cancellationToken) =>
        (await tagService.GetTagsAsync(search, cancellationToken)).ToResponse();

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Tag>> GetTag(TagId id, CancellationToken cancellationToken) =>
        (await tagService.GetTagAsync(id, cancellationToken)).ToResponse();

    [HttpGet("{slug}")]
    public async Task<ActionResult<Tag>> GetTag(string slug, CancellationToken cancellationToken) =>
        (await tagService.GetTagBySlugAsync(slug, cancellationToken)).ToResponse();

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<Tag>> PostTag([FromBody] CreateTag createTag, CancellationToken cancellationToken) =>
        (await tagService.CreateTagAsync(createTag, cancellationToken)).ToResponse();

    [HttpPut("{id:guid}")]
    [Authorize]
    public async Task<ActionResult<Tag>> PutTag(TagId id, [FromBody] CreateTag createTag, CancellationToken cancellationToken) =>
        (await tagService.UpdateTagAsync(id, createTag, cancellationToken)).ToResponse();

    [HttpDelete("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> DeleteTag(TagId id, CancellationToken cancellationToken) =>
        (await tagService.DeleteTagAsync(id, cancellationToken)).ToResponse();
}
