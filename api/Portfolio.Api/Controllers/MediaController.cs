using Microsoft.AspNetCore.Mvc;
using Portfolio.Api.Extensions;
using Portfolio.Api.Services.Image;

namespace Portfolio.Api.Controllers;

[ApiController]
[Route("[controller]")]
public sealed class MediaController(IImageService imageService) : ControllerBase
{
    [HttpGet("uploads/{fileName}")]
    public Task<IActionResult> GetFile(string fileName, CancellationToken cancellationToken) =>
        imageService.GetImageAsync(fileName, cancellationToken).ToFileResponse();
}
