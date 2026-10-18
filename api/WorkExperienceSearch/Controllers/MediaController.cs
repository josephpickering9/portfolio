using Microsoft.AspNetCore.Mvc;
using Work_Experience_Search.Extensions;
using Work_Experience_Search.Services.Image;

namespace Work_Experience_Search.Controllers;

[ApiController]
[Route("[controller]")]
public sealed class MediaController(IImageService imageService) : ControllerBase
{
    [HttpGet("uploads/{fileName}")]
    public async Task<IActionResult> GetFile(string fileName, CancellationToken cancellationToken) =>
        (await imageService.GetImageAsync(fileName, cancellationToken)).ToFileResponse();
}
