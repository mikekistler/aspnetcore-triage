using Microsoft.AspNetCore.Mvc;

namespace issue_60867.Controllers;

[ApiController]
[Route("[controller]")]
public class UploadController : ControllerBase
{
    [HttpPost("image-to-data")]
    [Consumes("multipart/form-data")]
    public ActionResult<ImageUploadResponse> ImageToData(
        [FromForm] IFormFile image
    )
    {
        var response = new ImageUploadResponse(image.FileName, image.Length);
        return Ok(response);
    }
}

public sealed record ImageUploadResponse(string FileName, long Length);
