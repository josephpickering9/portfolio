using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Work_Experience_Search.Extensions;
using Work_Experience_Search.Services;

namespace Work_Experience_Search.Controllers;

[ApiController]
[Route("[controller]")]
public sealed class ProjectImageController(IProjectImageService projectImageService) : ControllerBase
{
    [HttpPut("optimise")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public Task<IActionResult> OptimiseImages(CancellationToken cancellationToken) =>
        projectImageService.OptimiseImagesAsync(cancellationToken).ToNoContentOrError();
}
