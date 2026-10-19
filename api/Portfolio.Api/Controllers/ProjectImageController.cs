using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Api.Extensions;
using Portfolio.Api.Services;

namespace Portfolio.Api.Controllers;

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
